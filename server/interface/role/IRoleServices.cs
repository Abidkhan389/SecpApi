using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface IRoleService
    {
        Task<IResponse> GetAll();
        Task<IResponse> AddEdit(AddEditRole addEdit, AuditTrack audit);
        Task<IResponse> GetSingle(string id);
        Task<IResponse> GetAllByProc(TableParamModel model, int diff);
        Task<IResponse> ActiveInactive(ActiveInactiveRole model, AuditTrack audit);
    }
}
