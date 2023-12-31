using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradigm.Contract.Interface;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using static Paradigm.Data.Model.Lecture;

namespace Paradigm.Server.Application
{
    [Authorize]
    [Route("api/[controller]")]
    public class LectureController : Server.ControllerBase
    {

        private readonly ILetureService _lectureservice;
        private readonly IResponse _response;
        public LectureController(IDomainContextResolver resolver, ILocalizationService localization, ILetureService letureService, IResponse response) : base(resolver, localization)
        {

            this._lectureservice = letureService;
            this._response = response;
        }
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_Lectures model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var headers = this.Request.Headers;
            int diffinTime = 0;
            if ((headers.ContainsKey(Constants.TimeZone)))
            {
                diffinTime = Convert.ToInt32(headers[Constants.TimeZone].ToString());
                diffinTime = diffinTime * -1;
                diffinTime = diffinTime * 60;
            }
            return await _lectureservice.GetAllByProc(model, diffinTime);
        }
        [HttpGet]
        [Route("GetLectureById")]
        public async Task<object> GetLectureById(Guid Id)
        {
            return await _lectureservice.GetLectureById(Id);
        }
        [HttpGet]
        [Route("GetLecturesBycourse")]
        public async Task<object> GetLecturesBycourse(Guid Id)
        {
            return await _lectureservice.GetLecturesBycourse(Id);
        }
        [HttpPost]
        [Route("AddEditLecture")]
        public async Task<object> AddEditLecture([FromBody] AddEditLecture model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _lectureservice.AddEditLecture(model, userId);
        }
        [HttpPost]
        [Route("ActiveInactive")]
        public async Task<object> ActiveInactive([FromBody] ActiveInactive model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _lectureservice.ActiveInactive(model);
        }
        [HttpGet]
        [Route("GetLectureWithContent")]
        public async Task<IResponse> GetLectureWithContent(Guid Id)
        {
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _lectureservice.GetLectureWithContent(Id, userId);
        }
    }
}