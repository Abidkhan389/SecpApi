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
    public class DashboardController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IDashboardService _dashboardService;
        private readonly IResponse _response;
         public DashboardController(IDomainContextResolver resolver, ILocalizationService localization, IDashboardService dashboardService, IResponse response) : base(resolver, localization)
        {

            this._dashboardService = dashboardService;
            this._response = response;
        }
        [HttpGet]
        [Route("GetOverViewForAdminDashboard")]
        public async Task<Object> GetOverViewForAdminDashboard()
        {
            return await _dashboardService.GetOverViewForAdminDashboard();
        }
    }
}