using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Paradigm.Contract.Interface;
using Paradigm.Server.Interface;
using Paradigm.Data;
using System.Threading.Tasks;
using System;
using Paradigm.Data.Model;

namespace Paradigm.Server.Application
{
    //[Authorize]
    [Route("api/[controller]")]
    public class CategoryController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICategoryService _CategoryService;
        public CategoryController(ICategoryService categoryService, IDomainContextResolver resolver, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            _CategoryService = categoryService;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        public async Task<object> GetAllCategories()
        {
            return await _CategoryService.GetAllCategories();
        }
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_Categories model)
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
            return await _CategoryService.GetAllByProc(model, diffinTime);
        }



        [HttpGet]
        [Route("GetCategoryById")]
        public async Task<object> GetCategoryById(Guid Id)
        {
            return await _CategoryService.GetCategoryById(Id);
        }

        [HttpPost]
        [Route("AddEditCategory")]
        public async Task<object> AddEditCategory([FromBody] AddEditCategory model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _CategoryService.AddEditCategory(model);
        }
        [HttpGet]
        [Route("GetEnrolledCategories")]
        public async Task<object> GetEnrolledCategories(Guid UserId)
        {
            return await _CategoryService.GetEnrolledCategories(UserId);
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
            return await _CategoryService.ActiveInactive(model);
        }
    }

}