using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paradigm.Contract.Interface;
using Paradigm.Data;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    [Route("api/[controller]")]
    public class AttachmentController : Server.ControllerBase
    {
        private readonly object balanceLock = new object();
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICryptoService _crypto;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGeneralService _general;
        public AttachmentController(IHttpContextAccessor httpContextAccessor, IGeneralService general, IDomainContextResolver resolver, ICryptoService crypto, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            _crypto = crypto;
            _httpContextAccessor = httpContextAccessor;
            _general = general;
        }
        [HttpPost]
        [Route("UploadFormData")]
        public async Task<object> UploadFormData()
        {
            var formData = await Request.ReadFormAsync();
            string checkPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "vapeimages");
            //Check Path
            if (!Directory.Exists(checkPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(checkPath);
            }
            if (ModelState.IsValid && formData.Files.Count > 0)
            {
                List<string> uploadedFileNames = new List<string>();
                // Process the images
                foreach (var image in formData.Files)
                {
                    if (image.Length > 0)
                    {
                        // Generate a unique file name or use the original file name
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        // Save the image to a directory
                        string filePath = Path.Combine(checkPath, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        uploadedFileNames.Add(fileName);
                    }
                }
                _response.Success = Constants.ResponseSuccess;
                _response.Message = "File Uploaded";
                _response.Data = uploadedFileNames;
                return Ok(_response);
            }
            _response.Success = Constants.ResponseFailure;
            _response.Message = "No File Selected";
            return Ok(_response);
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<object> UploadFile()
        {
            if (Request.ContentLength > 0)
            {
                //Get File Name and Type
                var filename = this.Request.Headers["X-File-Name"];
                var fileType = this.Request.Headers["X-File-Type"];
                var filePath = this.Request.Headers["X-File-Path"];

                //Change name file name
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                if (fileNameWithoutExtension.Length > 25)
                {
                    fileNameWithoutExtension = fileNameWithoutExtension.Substring(0, 25);
                }
                fileNameWithoutExtension = Regex.Replace(fileNameWithoutExtension, @"[^a-zA-Z0-9 _-]", "");
                fileNameWithoutExtension = fileNameWithoutExtension.Replace(" ", "");
                fileNameWithoutExtension = fileNameWithoutExtension.Replace(")", "").Replace("(", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
                fileNameWithoutExtension = fileNameWithoutExtension.Replace("&", "and");
                var fileNameWithTimestamp = fileNameWithoutExtension + DateTime.Now.Ticks + Path.GetExtension(filename);
                //Create path
                string pth = filePath.ToString();
                string checkPath = String.IsNullOrEmpty(pth) ? checkPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") : checkPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pth);

                //Check Path
                if (!Directory.Exists(checkPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(checkPath);
                }

                string path = null;
                if (String.IsNullOrEmpty(pth))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileNameWithTimestamp);
                }
                else
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pth, fileNameWithTimestamp);
                }

                //Create File
                using (var destinationStream = new BinaryWriter(new FileStream(path, FileMode.Create)))
                {
                    long? length = this.Request.ContentLength;
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[2048];
                    //int bytesRead = await httpRequestStream.ReadAsync(buffer, 0, buffer.Length);

                    readCount = await Request.Body.ReadAsync(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        destinationStream.Write(buffer, 0, readCount);
                        readCount = await Request.Body.ReadAsync(buffer, 0, bufferSize); ;
                    }
                }

                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var baseImageUrl = "";

                if (String.IsNullOrEmpty(pth))
                {
                    baseImageUrl = Path.Combine(baseUrl, fileNameWithTimestamp);
                }
                else
                {
                    baseImageUrl = Path.Combine(baseUrl, pth, fileNameWithTimestamp);
                }

                _response.Success = Constants.ResponseSuccess;
                _response.Message = "File Uploaded";
                _response.Data = baseImageUrl;
                return Ok(_response);
            }
            else
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = "No File Selected";
                return Ok(_response);
            }
        }

        [HttpGet]
        [Route("DownloadFile")]
        public async Task<object> DownloadFile(string filename, string folderName)
        {

            if (filename == null)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.FileNameRequired;
                return Ok(_response);
            }

            string path = null;
            if (String.IsNullOrEmpty(folderName))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);
            }
            else
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName, filename);
            }

            try
            {
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);
                var file = new FileContentResult(bytes, new MediaTypeHeaderValue("application/octet").ToString())
                {
                    FileDownloadName = filename
                };
                return file;
            }
            catch
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.FileNotFound;
                return Ok(_response);
            }

        }
        [HttpGet]
        [Route("DeleteFile")]
        public async Task<object> DeleteFile(string filename, string ReferenceType, string folderName)
        {
            if (filename == null)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.FileNameRequired;
                return Ok(_response);
            }

            string path = null;
            if (String.IsNullOrEmpty(folderName))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);
            }
            else
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName, filename);
            }

            try
            {
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);
                System.IO.File.Delete(path);
                _response.Success = Constants.ResponseSuccess;
                _response.Message = "Successfully Deleted";
                return Ok(_response);

            }
            catch
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.FileNotFound;
                return Ok(_response);
            }
        }
    }
}