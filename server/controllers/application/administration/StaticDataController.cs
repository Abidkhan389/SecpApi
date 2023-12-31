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
    public class StaticDataController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly IStaticDataService _StaticData;
        public StaticDataController(IStaticDataService StaticData, IDomainContextResolver resolver, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            _StaticData = StaticData;
        }


        [HttpPost]
        [Route("GetAllStaticDataParentByProc")]
        public async Task<object> GetAllStaticDataParentByProc([FromBody] TableParamModel model)
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
            return await _StaticData.GetAllStaticDataParentByProc(model, diffinTime);
        }

        [HttpPost]
        [Route("AddEditStaticDataParent")]
        public async Task<object> AddEditStaticDataParent([FromBody] AddEditStaticDataParent addEdit)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _StaticData.AddEditStaticDataParent(addEdit, AuditTrack());
        }

        [HttpGet]
        [Route("GetSingleStaticDataParent")]
        public async Task<object> GetSingleStaticDataParent(Guid id)
        {
            return await _StaticData.GetSingleStaticDataParent(id);
        }

        [HttpGet]
        [Route("GetStaticDataParent")]
        public async Task<object> GetStaticDataParent()
        {
            return await _StaticData.GetStaticDataParent();
        }

        [HttpGet]
        [Route("GetStaticDataByParentId")]
        public async Task<object> GetStaticDataByParentId(Guid parentId)
        {
            return await _StaticData.GetStaticDataByParentId(parentId);
        }

        [HttpGet]
        [Route("GetStaticDataByParentName")]
        public async Task<object> GetStaticDataByParentName(string parentName)
        {
            return await _StaticData.GetStaticDataByParentName(parentName, AuditTrack());
        }

        [HttpGet]
        [Route("GetMultipleStaticData")]
        public async Task<object> GetMultipleStaticData(string parentName)
        {
            return await _StaticData.GetMultipleStaticData(parentName, AuditTrack());
        }

        [HttpPost]
        [Route("AddEdit")]
        public async Task<object> AddEdit([FromBody] AddEditStaticData addEdit)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _StaticData.AddEdit(addEdit, AuditTrack());
        }

        [HttpGet]
        [Route("GetSingle")]
        public async Task<object> GetSingle(Guid id)
        {
            if (id == Guid.Empty)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.IdRequired;
                return Ok(_response);
            }
            return await _StaticData.GetSingle(id);
        }
    }
}
