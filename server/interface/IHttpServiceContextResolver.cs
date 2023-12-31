namespace Paradigm.Server.Interface
{
    using Microsoft.AspNetCore.Http;

    using Paradigm.Contract.Interface;
    using Paradigm.Contract.Model;
    
    public interface IHttpServiceContextResolver : IDomainContextResolver
    {
        IUser Resolve(HttpContext context, bool cache = true);
    }
}
