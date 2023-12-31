﻿namespace Paradigm.Server.Filters
{
    using System.Threading.Tasks;
    
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    using Paradigm.Contract.Interface;

    public class IdentityMappingFilter : IAsyncResourceFilter
    {
        private readonly ILocalAuthenticationService authService;

        public IdentityMappingFilter(ILoggerFactory logger, ILocalAuthenticationService authService)
        {
            this.authService = authService;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var identity = context.HttpContext.User.Identity;

            if (!string.IsNullOrWhiteSpace(identity.Name))
            {
                var user = await this.authService.ResolveUser(identity.Name);

                if (user != null)
                    context.HttpContext.Items.Add(Extensions.HttpContextCurrentUserKey, user);
            }

            await next.Invoke();
        }
    }
}
