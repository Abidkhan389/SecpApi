using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Paradigm.Contract.Interface;
using Paradigm.Data;
using Paradigm.Server.Interface;
using Paradigm.Data.Model;
using System;

namespace Paradigm.Server.Application
{
    [Authorize]
    [Route("api/[controller]")]
    public class AuditController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly IAuditService _audit;
        public AuditController(IAuditService audit, IDomainContextResolver resolver, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            _audit = audit;
        }

        [HttpPost]
        [Route("GetAllLoginAuditsByProc")]
        public async Task<object> GetAllLoginAuditsByProc([FromBody] TableParamModel model)
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
            return await _audit.GetAllLoginAuditsByProc(model, diffinTime);
        }
    }
}
