using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Paradigm.Contract.Interface;
using Paradigm.Data;
using Microsoft.EntityFrameworkCore;

namespace Paradigm.Server.Application
{
    [Authorize]
    [Route("api/[controller]")]
    public class GeneralController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        public GeneralController(IDomainContextResolver resolver, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetAll")]
        public async Task<object> GetAll()
        {
            var data = await _dbContext.AppException.ToListAsync();
            _response.Data = data;
            _response.Success = Constants.ResponseSuccess;
            _response.Message = Constants.DataSaved;
            return Ok(_response);
        }
    }
}
