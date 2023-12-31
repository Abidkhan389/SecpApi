namespace Paradigm.Server
{
    using Microsoft.AspNetCore.Mvc;

    using Paradigm.Service;
    using Paradigm.Server.Filters;
    using Paradigm.Contract.Interface;
    using Paradigm.Data.Model;
    using System;
    using Paradigm.Server.Application;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    [ServiceFilter(typeof(ApiResultFilter))]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [ServiceFilter(typeof(IdentityMappingFilter))]
    public class ControllerBase : Controller
    {
        protected ILocalizationService Localization { get; private set; }
        protected IDomainContextResolver Resolver;
        private ILocalizationDictionary dictionary;
        private IDomainContext domainContext;

        public ControllerBase(IDomainContextResolver resolver, ILocalizationService localization)
        {
            this.Localization = localization;
            this.Resolver = resolver;
        }

        protected ILocalizationDictionary Dictionary
        {
            get
            {
                if (this.dictionary == null)
                    this.dictionary = this.Localization.CreateDictionary(this.DomainContext);

                return this.dictionary;
            }
        }

        protected IDomainContext DomainContext
        {
            get
            {
                if (this.domainContext == null)
                    this.domainContext = this.Resolver.Resolve();

                return this.domainContext;
            }
        }

        protected void ThrowLocalizedServiceException(string key)
        {
            throw new ServiceException(this.Dictionary[key].Value);
        }
        protected AuditTrack AuditTrack()
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return new AuditTrack(Guid.Parse(DomainContext.User.UserId), DomainContext.User.Username, HelperStatic.GetURL(this.Request.Headers), HelperStatic.GetAuditSessionIdFromClaims((ClaimsIdentity)User.Identity), options, HelperStatic.GetCurrentTimeStamp());
        }
    }
}
