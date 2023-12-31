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
    public class ProfileController : Server.ControllerBase
    {
        private readonly IResponse _response;
        private readonly IUserProfileService _userProfile;
        public ProfileController(IDomainContextResolver resolver, ILocalizationService localization, IResponse response,
                             IUserProfileService userProfile) : base(resolver, localization)
        {
            this._response = response;
            this._userProfile = userProfile;
        }

        [HttpGet]
        [Route("UserProfile")]
        public async Task<object> UserProfile(Guid UserId)
        {
            return await _userProfile.UserProfile(UserId);
        }
        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<object> GetUserDetails(Guid UserId)
        {
            return await _userProfile.GetUserDetails(UserId);
        }
        [HttpGet]
        [Route("GetUserCertificate")]
        public async Task<object> GetUserCertificate(Guid UserId)
        {
            return await _userProfile.GetUserCertificate(UserId);
        }

    }
}