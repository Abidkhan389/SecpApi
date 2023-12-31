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
    public class MenuController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly IMenuService _menu;
        public MenuController(IMenuService menu, IDomainContextResolver resolver, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            _menu = menu;
        }

        #region Menu Mapping
        [HttpGet]
        [Route("GetScreensByRole")]
        public async Task<object> GetScreensByRole(string roleId)
        {
            if (String.IsNullOrEmpty(roleId))
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.IdRequired;
                return Ok(_response);
            }
            return await _menu.GetScreensByRole(roleId);
        }

        [HttpPost]
        [Route("MapScreensToRole")]
        public async Task<object> MapScreensToRole([FromBody] ScreenRoleMap model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _menu.MapScreensToRole(model);
        }

        #endregion

        #region Menu
        [HttpGet]
        [Route("GetMenu")]
        public async Task<object> GetMenu()
        {
            return await _menu.GetMenu(AuditTrack());
        }
        #endregion

    }
}
