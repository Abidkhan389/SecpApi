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
    public class GradeController : Server.ControllerBase
    {
        private readonly IResponse _response;
        private readonly IGradeService _gradeService;
        public GradeController(IDomainContextResolver resolver, ILocalizationService localization, IResponse response,
                             IGradeService gradeService) : base(resolver, localization)
        {
            this._response = response;
            this._gradeService = gradeService;
        }
        [HttpPost]
        [Route("GetLectureWiseGrade")]
        public async Task<object> GetLectureWiseGrade([FromBody] QuizDetail model)
        {
            return await _gradeService.GetLectureWiseGrade(model);
        }
        [HttpPost]
        [Route("GetCourseWiseGrade")]
        public async Task<object> GetCourseWiseGrade([FromBody] AssessmentDetail model)
        {
            return await _gradeService.GetCourseWiseGrade(model);
        }
        [HttpGet]
        [Route("GetUserGrades")]
        public async Task<object> GetUserGrades(Guid UserId)
        {
            return await _gradeService.GetUserGrades(UserId);
        }
             [HttpGet]
        [Route("GetUserGradeForCourse")]
        public async Task<object> GetUserGradeForCourse(Guid CourseId,Guid UserId)
        {
            return await _gradeService.GetUserGradeForCourse(CourseId,UserId);
        }
    }
}