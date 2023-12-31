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
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class RoleController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly IRoleService _Role;
        public RoleController(IRoleService Role, IDomainContextResolver resolver, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            _Role = Role;
        }

        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<object> GetAllRoles()
        {
            return await _Role.GetAll();
        }
        
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] TableParamModel model)
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
            return await _Role.GetAllByProc(model, diffinTime);
        }

        [HttpPost]
        [Route("AddEdit")]
        public async Task<object> AddEdit([FromBody] AddEditRole addEdit)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var user = Guid.Parse(DomainContext.User.UserId);
            return await _Role.AddEdit(addEdit, AuditTrack());
        }

        [HttpGet]
        [Route("GetSingle")]
        public async Task<object> GetSingle(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.IdRequired;
                return Ok(_response);
            }
            return await _Role.GetSingle(id);
        }

        [HttpPost]
        [Route("ActiveInactive")]
        public async Task<object> ActiveInactive([FromBody] ActiveInactiveRole model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _Role.ActiveInactive(model, AuditTrack());
        }
    }
}
