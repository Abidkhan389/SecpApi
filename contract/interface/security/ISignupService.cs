namespace Paradigm.Contract.Interface
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Paradigm.Contract.Model;

    public interface ISignupService
    {
        Task<ClaimsIdentity> SignupUser(ISignupServiceOptions options);
        Task<ClaimsIdentity> RedeemVerificationCode(IUser user, string code);
        Task<string> SendVerificationCode(IUser user, string providerKey);
    }
}
