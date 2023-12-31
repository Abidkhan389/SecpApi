using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Paradigm.Contract.Interface;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Server.ControllerBase
    {
        private DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICryptoService _crypto;
        private readonly IUserService _user;
        public UserController(IUserService user, IDomainContextResolver resolver, ICryptoService crypto, ILocalizationService localization, IResponse resp, DbContextBase dbContext) : base(resolver, localization)
        {
            _dbContext = dbContext;
            _response = resp;
            _crypto = crypto;
            _user = user;
        }

        #region User CRUD
        [HttpPost]
        [Route("GetAllByProc")]
        public async Task<object> GetAllByProc([FromBody] List_User model)
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
            return await _user.GetAllByProc(model, diffinTime);
        }

        [HttpPost]
        [Route("AddEdit")]
        public async Task<object> AddEdit([FromBody] AddEditUser addEdit)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _user.AddEdit(addEdit, AuditTrack());
        }

        [HttpGet]
        [Route("GetSingle")]
        public async Task<object> GetSingle(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.IdRequired;
                return Ok(_response);
            }
            return await _user.GetSingle(Id);
        }
         [HttpGet]
        [Route("GetUserDetailForCertificate")]
        public async Task<object> getUserDetailForCertificate(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.IdRequired;
                return Ok(_response);
            }
            return await _user.getUserDetailForCertificate(Id);
        }

        [HttpPost]
        [Route("ActiveInactive")]
        public async Task<object> ActiveInactive([FromBody] ActiveInactiveBool model)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _user.ActiveInactive(model, AuditTrack());
        }

        #endregion
        //Edit User Profile
        [HttpPost]
        [Route("EditProfile")]
        public async Task<object> EditProfile([FromBody] EditProfile editUser)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            return await _user.EditProfile(editUser, AuditTrack());
        }
        [HttpGet]
        [Route("ResetDefaultPassword")]
        public async Task<object> ResetDefaultPassword(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                return Ok(_response);
            }
            var salt = _crypto.CreateSalt();
            var hash = _crypto.CreateKey(salt, password);
            user.PasswordSalt = salt;
            user.PasswordHash = hash;
            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return Ok(_response);
        }

    }

}
