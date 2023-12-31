using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface IStaticDataService
    {
        Task<IResponse> GetAllStaticDataParentByProc(TableParamModel model, int diff);
        Task<IResponse> GetStaticDataParent();
        Task<IResponse> GetStaticDataByParentId(Guid parentId);
        Task<IResponse> GetStaticDataByParentName(string parentName, AuditTrack audit);
        Task<IResponse> GetMultipleStaticData(string parentName, AuditTrack audit);
        Task<IResponse> AddEditStaticDataParent(AddEditStaticDataParent addEdit, AuditTrack audit);
        Task<IResponse> GetSingleStaticDataParent(Guid id);
        Task<IResponse> AddEdit(AddEditStaticData addEdit, AuditTrack audit);
        Task<IResponse> GetSingle(Guid id);
    }
}
