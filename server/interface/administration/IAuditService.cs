using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface IAuditService
    {
        Task<IResponse> GetAllLoginAuditsByProc(TableParamModel model, int diff);
        Task<object> AddOne(AuditTrails entity);
    }
}