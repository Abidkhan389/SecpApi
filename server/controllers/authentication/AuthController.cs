namespace Paradigm.Server.Authentication
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Antiforgery;

    using Paradigm.Server;
    using Paradigm.Service;
    using Paradigm.Server.Model;
    using Paradigm.Service.Security;
    using Paradigm.Service.Repository;
    using Paradigm.Contract.Interface;
    using Paradigm.Server.Application;
    using Paradigm.Data;
    using Microsoft.EntityFrameworkCore;
    using Paradigm.Server.Interface;
    using Paradigm.Data.Model;

    [Route("api/auth/[action]")]
    public class AuthController : Server.ControllerBase
    {
        private readonly IAntiforgery antiForgeryService;
        private readonly IUserRepository repository;
        private readonly CultureService cultureService;
        private readonly IOptions<AppConfig> config1;
        private readonly IDomainContextResolver resolver;
        private readonly ILocalizationService localization;
        private readonly IUserProfileService _userProfile;
        private readonly ITokenProviderService<Token> tokenService;
        private readonly AppConfig config;
        private readonly IResponse _response;
        private readonly DbContextBase _dbContext;
        private readonly IAuditService _audit;
        private readonly LogInResponse _loginResponse;
        private readonly ICryptoService _crypto;

        public AuthController(IAuditService audit, DbContextBase dbContext, IResponse response, IAntiforgery antiForgeryService,
         IUserRepository repository, ITokenProviderService<Token> tokenService, CultureService cultureService,
          IOptions<AppConfig> config, IDomainContextResolver resolver,
           ILocalizationService localization, IUserProfileService userProfile, ICryptoService crypto) : base(resolver, localization)
        {
            this.antiForgeryService = antiForgeryService;
            this.repository = repository;
            this.tokenService = tokenService;
            this.cultureService = cultureService;
            config1 = config;
            this.resolver = resolver;
            this.localization = localization;
            this._userProfile = userProfile;
            this.config = config.Value;
            this._response = response;
            this._dbContext = dbContext;
            this._audit = audit;
            this._loginResponse = new LogInResponse();
            this._crypto = crypto;
        }

        [HttpGet]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> SampleLogin()
        {
            //Sample Values
            AuditTrails trails = new AuditTrails() { Location = "KHAZANA", Country = "PK", City = "LHR", DeviceType = "LAPTOP", IPAddress = "122.233.234.23", Time = HelperStatic.GetCurrentTimeStamp(), OperatingSystem = "WINDOWS", UserId = null, UserName = "admin@paradigm.org", AuditType = null, AuditSessionID = null };
            TokenRequest credentials = new TokenRequest() { Username = "admin@paradigm.org", Password = "P@ssw0rd" };
            return Ok(await this.Login(credentials));
        }

        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> Login([FromBody] TokenRequest credentials)
        {
            if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var identity = await repository.ResolveUser(credentials.Username, credentials.Password, false);
            var user = await repository.ResolveUser(credentials.Username);

            if (identity == null || user == null || user.Enabled==false)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.IncorrectUsernamePassword;
                return Ok(_response);
            }
            Guid userId = Guid.Parse(user.UserId);
            UserDetail userdetail = await _dbContext.UserDetail.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userdetail == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "UserDetail");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }

            this.SetAntiforgeryCookies();

            string cultureClaimKey = this.config.Service.ClaimsNamespace + ProfileClaimTypes.CultureName;
            string timeZoneIdKey = this.config.Service.ClaimsNamespace + ProfileClaimTypes.TimeZoneId;

            string cultureName = identity.Claims.FirstOrDefault(o => o.Type == cultureClaimKey).Value;
            string timeZoneId = identity.Claims.FirstOrDefault(o => o.Type == timeZoneIdKey).Value;

            AuditTrails trails = new AuditTrails() { Location = "KHAZANA", Country = "PK", City = "LHR", DeviceType = "LAPTOP", IPAddress = "122.233.234.23", Time = HelperStatic.GetCurrentTimeStamp(), OperatingSystem = "WINDOWS", UserId = null, UserName = "admin@paradigm.org", AuditType = null, AuditSessionID = null };
            trails.AuditType = Constants.ActionMethods.LoginSuccess;
            trails.UserId = userId;
            var res = await this._audit.AddOne(trails);
            string resStr = res.ToString();
            string sessionId = resStr.Split("*")[1];

            this.cultureService.RefreshCookie(this.HttpContext, cultureName, timeZoneId);

            var token = await this.tokenService.IssueToken(identity, identity.Name, sessionId, userId.ToString());
           this._loginResponse.Token = token.access_token;
           this._loginResponse.Id = userId;
           this._loginResponse.UserName=user.Username;
            _response.Data = _loginResponse;
            _response.Success = Constants.ResponseSuccess;
            return Ok(_response);
        }

        [HttpPut()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<Object> Logout()
        {
            string cookieName = this.config.Server.AntiForgery.CookieName;

            if (this.HttpContext.Request.Cookies[cookieName] != null)
                this.HttpContext.Response.Cookies.Delete(cookieName);

            string clientName = this.config.Server.AntiForgery.ClientName;

            if (this.HttpContext.Request.Cookies[clientName] != null)
                this.HttpContext.Response.Cookies.Delete(clientName);

            return await Task.FromResult(true);
        }

        private void ClearAntiforgeryCookies()
        {
            string cookieName = this.config.Server.AntiForgery.CookieName;

            if (this.HttpContext.Request.Cookies[cookieName] != null)
                this.HttpContext.Response.Cookies.Delete(cookieName);

            string clientName = this.config.Server.AntiForgery.ClientName;

            if (this.HttpContext.Request.Cookies[clientName] != null)
                this.HttpContext.Response.Cookies.Delete(clientName);
        }

        private void SetAntiforgeryCookies()
        {
            var context = this.HttpContext;
            var tokenSet = antiForgeryService.GetAndStoreTokens(context);

            if (tokenSet.RequestToken != null)
            {
                string clientName = this.config.Server.AntiForgery.ClientName;
                context.Response.Cookies.Append(clientName, tokenSet.RequestToken, new CookieOptions() { HttpOnly = false, Secure = true });
            }
        }
        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> ResetPassword([FromBody] ResetPassword model)
        {
             if (!ModelState.IsValid)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.ModelStateStateIsInvalid;
                return Ok(_response);
            }
            var username= await _dbContext.User.FirstOrDefaultAsync(x=> x.UserId == model.UserId);
            var user = await repository.ResolveUser(username.Username,model.OldPassword, false);
            if (user == null)
            {
                _response.Message = ("No Password Match");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            var passswordSalt = _crypto.CreateSalt();
            var passwordHash = _crypto.CreateKey(passswordSalt,model.Password);
            //model.NewPassword= passwordHash;
            return await _userProfile.ResetPassword(model, AuditTrack(), passwordHash,passswordSalt);
        }

    }
}
