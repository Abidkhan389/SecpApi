using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class AssessmentController : Server.ControllerBase
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IResponse _response;

        public AssessmentController(IAssessmentService assessmentService,
            IDomainContextResolver resolver, ILocalizationService localization, IResponse resp) : base(resolver, localization)
        {
            this._assessmentService = assessmentService;
            this._response = resp;
        }

        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_Assessment model)
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
            return await _assessmentService.GetAllByProc(model, diffinTime);
        }
        [HttpGet]
        [Route("GetAssessmentQuestionById")]
        public async Task<object> GetAssessmentQuestionById(Guid Id)
        {
            return await _assessmentService.GetAssessmentQuestionById(Id);
        }
        [HttpPost]
        [Route("AddEditAssessment")]
        public async Task<Object> AddEditAssessment([FromBody] AddEditAssessment model)
        {
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _assessmentService.AddEditAssessment(model, userId);
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
            return await _assessmentService.ActiveInactive(model);
        }
        [HttpGet]
        [Route("GetAssessmentQuestions")]
        public async Task<object> GetAssessmentQuestions(Guid Id)
        {
            return await _assessmentService.GetAssessmentQuestions(Id);
        }

    }
}
