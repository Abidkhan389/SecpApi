namespace Paradigm.Contract.Interface
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Paradigm.Contract.Model;

    public interface ILocalAuthenticationService
    {
        Task<ClaimsIdentity> ResolveUser(string username, string password, bool isSuperAdmin);
        Task<IUser> ResolveUser(string username);
        Task<bool> ValidateUser(string username);
    }
}
