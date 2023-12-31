using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradigm.Contract.Interface;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    [Authorize]
    [Route("api/[controller]")]
    public class GradingCriteriaController : Server.ControllerBase
    {
        private readonly IResponse _response;
        private readonly IGradingCriteria _iGradingCriteria;

        public GradingCriteriaController(IDomainContextResolver resolver, ILocalizationService localization, IResponse response
                            ,IGradingCriteria iGradingCriteria  ) : base(resolver, localization)
        {
            this._response = response;
            this._iGradingCriteria = iGradingCriteria;
        }
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_GradingCriteria model)
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
            return await _iGradingCriteria.GetAllByProc(model, diffinTime);
        }



        [HttpGet]
        [Route("GetGradingCriteriaById")]
        public async Task<object> GetGradingCriteriaById(Guid Id)
        {
            return await _iGradingCriteria.GetGradingCriteriaById(Id);
        }

        [HttpPost]
        [Route("AddEditGradingCriteria")]
        public async Task<object> AddEditGradingCriteria([FromBody] VW_AddEditGradingCriteria model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var userId = HelperStatic.GetUserIdFromClaims((ClaimsIdentity)User.Identity);
            return await _iGradingCriteria.AddEditGradingCriteria(model,userId);
        }
        
    }
}