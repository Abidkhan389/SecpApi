using System;
using System.Threading.Tasks;
using Paradigm.Server.Application;
using Paradigm.Data.Model;

namespace Paradigm.Server.Interface
{
    public interface IGradingCriteria
    {
        Task<IResponse> GetAllByProc(List_GradingCriteria model, int diff);
        Task<IResponse> GetGradingCriteriaById(Guid id);
        Task<IResponse> AddEditGradingCriteria(VW_AddEditGradingCriteria model, Guid userId);
    }
}