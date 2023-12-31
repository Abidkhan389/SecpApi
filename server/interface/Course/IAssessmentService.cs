using System;
using Paradigm.Data.Model;
using Paradigm.Server.Application;
using System.Threading.Tasks;

namespace Paradigm.Server.Interface
{
    public interface IAssessmentService
    {
        Task<IResponse> GetAllByProc(List_Assessment model, int diff);
        Task<IResponse> AddEditAssessment(AddEditAssessment model, Guid Id);
        Task<IResponse> GetAssessmentQuestionById(Guid Id);
        Task<IResponse> ActiveInactive(ActiveInactive model);
        Task<IResponse> GetAssessmentQuestions(Guid Id);
    }
}