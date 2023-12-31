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
    public class EnrollmentController : Server.ControllerBase
    {
        private readonly IResponse _response;
        private readonly IEnrollmentService _enrollment;
        public EnrollmentController(IDomainContextResolver resolver, ILocalizationService localization, IResponse response,
                             IEnrollmentService enrollment) : base(resolver, localization)
        {
            this._response = response;
            this._enrollment = enrollment;
        }
        [HttpPost]
        [Route("EnrollUser")]
        public async Task<object> EnrollUser([FromBody] VW_EnrolledViewModel model)
        {
            return await _enrollment.EnrollUser(model);
        }
        [HttpGet]
        [Route("GetLearningOverViewCount")]
        public async Task<object> GetLearningOverViewCount(Guid UserId)
        {
            // var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);

            // UserId = UserId.Remove(UserId.Length - 1);
            // UserId = UserId.Remove(0, 1);


            return await _enrollment.GetLearningOverViewCount(UserId);

        }
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<object> GetAllUsers()
        {
            return await _enrollment.GetAllUsers();
        }
        [HttpGet]
        [Route("GetCoursesByCategory")]
        public async Task<object> GetCoursesByCategory(Guid Id)
        {
            return await _enrollment.GetAllCoursesByCategoryId(Id);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<object> GetById(Guid Id)
        {
            return await _enrollment.GetById(Id);
        }
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_EnrolledUser model)
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
            return await _enrollment.GetAllByProc(model, diffinTime);
        }
        [HttpPost]
        [Route("AddEditEnrollment")]
        public async Task<object> AddEditEnrollment([FromBody] AddEditEnrollment model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _enrollment.AddEditEnrollment(model, userId);
        }
        [HttpPost]
        [Route("GetAllBywaitingForApprovalUserList")]
        public async Task<object> GetAllBywaitingForApprovalUserList([FromBody] List_UserApproval model)
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
            return await _enrollment.GetAllBywaitingForApprovalUserList(model, diffinTime);
        }
        [HttpPost]
        [Route("GetAllAgainRequestsForCourseApproval")]
        public async Task<object> GetAllAgainRequestsForCourseApproval([FromBody] List_UserApproval model)
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
            return await _enrollment.GetAllAgainRequestsForCourseApproval(model, diffinTime);
        }
        [HttpGet]
        [Route("ApproveUserRequest")]
        public async Task<object> ApproveUserRequest(Guid Id)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _enrollment.ApproveUserRequest(Id, userId);
        }
    }
}