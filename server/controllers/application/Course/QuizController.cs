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
    public class QuizController : Server.ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly DbContextBase _dbContext;
        private readonly IDomainContextResolver resolver;
        private readonly ILocalizationService localization;
        private readonly IResponse _response;

        public QuizController(IQuizService quizService, DbContextBase dbContext,
            IDomainContextResolver resolver, ILocalizationService localization, IResponse resp) : base(resolver, localization)
        {
            this._quizService = quizService;
            this._dbContext = dbContext;
            this.resolver = resolver;
            this.localization = localization;
            this._response = resp;
        }

        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<object> GetAllCourses()
        {
            return await _quizService.GetAllCourses();
        }

        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_Quiz model)
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
            return await _quizService.GetAllByProc(model, diffinTime);
        }
        
        [HttpGet]
        [Route("GetAllLectures")]
        public async Task<object> GetAllLectures(Guid Id )
        {
            return await _quizService.GetAllLectures(Id);
        }

        [HttpGet]
        [Route("GetQuestionById")]
        public async Task<object> GetQuestionById(Guid Id)
        {
            return await _quizService.GetQuestionById(Id);
        }
        [HttpPost]
        [Route("AddEditQuestion")]
        public async Task<Object> EditQuestion([FromBody] VW_Quizaddedit model)
        {
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _quizService.AddEditQuestion(model, userId);
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
            return await _quizService.ActiveInactive(model);
        }
        //Get All Question for specific course and for specific lecture for User Portal
        [HttpGet]
        [Route("GetQuestionsByLecture")]
        public async Task<object> GetQuestionsByLecture(Guid courseId, Guid lectureId)
        {
            return await _quizService.GetQuestionsByLecture(courseId, lectureId);
        }
    }
}
