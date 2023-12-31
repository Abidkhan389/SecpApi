using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradigm.Contract.Interface;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Application;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    [Authorize]
    [Route("api/[controller]")]
    public class CourseContentController : Server.ControllerBase
    {
        private readonly ICourseContentService _courseContentService;
        private readonly DbContextBase _dbContext;
        private readonly IDomainContextResolver resolver;
        private readonly ILocalizationService localization;
        private readonly IResponse _response;

        public CourseContentController(IResponse resp, ICourseContentService courseContentService, DbContextBase dbContext, IDomainContextResolver resolver, ILocalizationService localization) : base(resolver, localization)
        {
            this._courseContentService = courseContentService;
            this._dbContext = dbContext;
            this.resolver = resolver;
            this.localization = localization;
            this._response = resp;
        }
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_Content model)
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
            return await _courseContentService.GetAllByProc(model, diffinTime);
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
            return await _courseContentService.ActiveInactive(model);
        }

        [HttpGet]
        [Route("GetCourseContentById")]
        public async Task<object> GetCourseContentById(Guid Id)
        {
            return await _courseContentService.GetCourseContentById(Id);
        }
        [HttpGet]
        [Route("GetAllCoursesForTraining")]
        public async Task<object> GetAllCoursesForTraining()
        {
            return await _courseContentService.GetAllCoursesForTraining();
        }
        [HttpGet]
        [Route("GetContentforCourse")]
        public async Task<object> GetContentforCourse(Guid Id)
        {
            return await _courseContentService.GetContentforCourse(Id);
        }
        [HttpGet]
        [Route("GetLecturesForCourse")]
        public async Task<object> GetLecturesForCourse(Guid Id)
        {
            return await _courseContentService.GetLecturesForCourse(Id);
        }
        [HttpPost]
        [Route("AddEditCourseContent")]
        public async Task<object> AddEditCourseContent([FromBody] AddEditContent model)
        {
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _courseContentService.AddEditCourseContent(model, userId);

        }
    }
}