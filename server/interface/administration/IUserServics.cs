using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface IUserService
    {
        Task<IResponse> AddEdit(AddEditUser addEdit, AuditTrack audit);
        Task<IResponse> EditProfile(EditProfile editUser, AuditTrack audit);
        Task<IResponse> GetSingle(Guid id);
        Task<IResponse> getUserDetailForCertificate(Guid id);
        Task<IResponse> GetAllByProc(List_User model, int diff);
        Task<IResponse> ActiveInactive(ActiveInactiveBool model, AuditTrack audit);
    }
}
