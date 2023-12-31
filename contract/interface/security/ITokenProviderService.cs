namespace Paradigm.Contract.Interface
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface ITokenProviderService<T>
    {
        Task<T> IssueToken(ClaimsIdentity identity, string subject, string sessionId, string userId);
    }
}