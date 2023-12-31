using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradigm.Contract.Interface;
using Paradigm.Data.Model;
using Paradigm.Server.Application;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    [Authorize]
    [Route("api/[controller]")]
    public class CourseController : Server.ControllerBase
    {

        private readonly ICourseService _courseService;
        private readonly IResponse _response;
        public CourseController(IDomainContextResolver resolver, ILocalizationService localization, ICourseService courseService, IResponse response) : base(resolver, localization)
        {

            this._courseService = courseService;
            this._response = response;
        }
        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<object> GetAllCourses()
        {
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _courseService.GetAllCourses(userId);
        }
        [HttpGet]
        [Route("GetEnrolledCourses")]
        public async Task<object> GetEnrolledCourses(Guid UserId)
        {
            return await _courseService.GetEnrolledCourses(UserId);
        }
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_Courses model)
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
            return await _courseService.GetAllByProc(model, diffinTime);
        }
        [HttpGet]
        [Route("GetCourseById")]
        public async Task<object> GetCourseById(Guid Id)
        {
            return await _courseService.GetCourseById(Id);

        }
        [HttpGet]
        [Route("GetCoursewithLectures")]
        public async Task<object> GetCoursewithLectures(Guid Id)
        {
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _courseService.GetCoursewithLectures(Id, userId);
        }
        [HttpGet]
        [Route("GetEnrolledCoursesByCategoryId")]
        public async Task<object> GetEnrolledCoursesByCategoryId(Guid CategoryId, Guid UserId)
        {
            return await _courseService.GetEnrolledCoursesByCategoryId(CategoryId, UserId);

        }
        [HttpGet]
        [Route("GetCoursesByCategoryId")]
        public async Task<object> GetCoursesByCategoryId(Guid CategoryId, Guid UserId)
        {
            return await _courseService.GetCoursesByCategoryId(CategoryId, UserId);

        }
        [HttpGet]
        [Route("getAllCategoriesForCourses")]
        public async Task<object> getAllCategoriesForCourses()
        {

            return await _courseService.GetAllCategoriesForCourses();

        }

        [HttpPost]
        [Route("AddEditCourse")]
        public async Task<object> AddEditCourse([FromBody] VW_Courseaddedit model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _courseService.AddEditCourse(model, userId);
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
            return await _courseService.ActiveInactive(model);
        }
    }
}