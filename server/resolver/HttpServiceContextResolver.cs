namespace Paradigm.Server.Resolver
{

    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    
    using Paradigm.Server;
    using Paradigm.Service;
    using Paradigm.Service.Model;
    using Paradigm.Contract.Model;
    using Paradigm.Server.Interface;    
    using Paradigm.Contract.Interface;    

    public class HttpServiceContextResolver : IHttpServiceContextResolver
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly Server.Config config;
        private readonly CultureService culture;
        private readonly HttpServiceContextFactory factory;
        private User cacheUser;

        public HttpServiceContextResolver(IOptions<Server.Config> config, IHttpContextAccessor contextAccessor, CultureService culture, HttpServiceContextFactory factory)
        {
            this.config = config.Value;
            this.contextAccessor = contextAccessor;
            this.culture = culture;
            this.factory = factory;
        }

        public IUser Resolve(HttpContext context, bool cache = true)
        {
            User user = cacheUser;

            if (user == null)
            {
                user = context.User.FromClaimsPrincipal();

                if (user == null)
                {
                    user = new User()
                    {
                        CultureName = this.culture.GetFromRequest(context, CultureService.CultureName),
                        DisplayName = null,
                        Enabled = context.User.Identity.IsAuthenticated,
                        Username = context.User.Identity.Name,
                        UserId = new Guid().ToString(),
                        TimeZoneId = this.culture.GetFromRequest(context, CultureService.TimeZoneId)
                    };
                }
                else
                {
                    user.CultureName = this.culture.GetFromRequest(context, CultureService.CultureName);
                    user.TimeZoneId = this.culture.GetFromRequest(context, CultureService.TimeZoneId);
                }

                if (string.IsNullOrEmpty(user.CultureName))
                    user.CultureName = this.config.DefaultCulture;

                if (string.IsNullOrEmpty(user.TimeZoneId))
                    user.TimeZoneId = this.config.DefaultTimeZone;

                if (cache)
                    cacheUser = user;
            }

            return user;
        }

        public IDomainContext Resolve(bool cache = true)
        {
            IUser user = this.Resolve(this.contextAccessor.HttpContext, cache);

            return this.factory.Create(user);
        }
    }
}
