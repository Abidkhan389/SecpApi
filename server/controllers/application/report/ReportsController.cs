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
    [Authorize]
    [Route("api/[controller]")]
    public class ReportsController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly IEnrollmentService _iEnrollmentService;
        private readonly IEnrolledCourseReport _enrollmentReport;
        private readonly ICompletedCoursesGradeReport _completedCourseGrade;
        public ReportsController(IEnrolledCourseReport enrollmentReport, 
        ICompletedCoursesGradeReport completedCourseGrade, IDomainContextResolver resolver,
         ILocalizationService localization, IResponse resp,IEnrollmentService iEnrollmentService,
          DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            this._iEnrollmentService = iEnrollmentService;
            _enrollmentReport = enrollmentReport;
            _completedCourseGrade = completedCourseGrade;

        }

        [HttpPost]
        [Route("EnrolledUserDetails")]
        public async Task<object> GetEnrollmentDetailsReport([FromBody] EnrolledCourse model)
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
            return await _enrollmentReport.GetEnrolledUserReport(model, diffinTime);
        }
        [HttpPost]
        [Route("EnrolledCoursesCountReport")]
        public async Task<object> GetEnrolledCoursesCountReport([FromBody] EnrolledCourseCount model)
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
            return await _enrollmentReport.GetEnrolledCourseCount(model, diffinTime);
        }
        [HttpPost]
        [Route("GetAllUserGrades")]
        public async Task<object> GetAllUserGrades([FromBody] CompletedCourseGrade model)
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
            return await _completedCourseGrade.GetAllUserGrades(model, diffinTime);
        }
        [HttpGet]
        [Route("GetGradeById")]
        public async Task<object> GetGradeById(Guid Id)
        {
            return await _completedCourseGrade.GetGradeById(Id);
        }

        [HttpPost]
        [Route("updateEnrollmentDisableStatus")]
        public async Task<object> updateEnrollmentDisableStatus([FromBody] ActiveInactive model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _iEnrollmentService.updateEnrollmentDisableStatus(model);
        }
    }

}