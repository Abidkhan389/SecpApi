using System;
using System.Threading.Tasks;
using Paradigm.Server.Application;
using Paradigm.Data.Model;

namespace Paradigm.Server.Interface
{
    public interface IUserProfileService
    {
        Task<IResponse> UserProfile(Guid UserId);
        Task<IResponse> GetUserDetails(Guid UserId);
        Task<IResponse> GetUserCertificate (Guid UserId);
        Task<IResponse> ResetPassword(ResetPassword model, AuditTrack audit, string passwordHash, string PasswordSalt);
    }
}
