using System;
using System.Threading.Tasks;
using Paradigm.Server.Application;
using Paradigm.Data.Model;

namespace Paradigm.Server.Interface
{
    public interface IEnrollmentService
    {
        Task<IResponse> EnrollUser(VW_EnrolledViewModel model);
        Task<IResponse> GetLearningOverViewCount(Guid UserId);
        Task<IResponse> GetAllUsers();
        Task<IResponse> GetAllByProc(List_EnrolledUser model, int diff);
        Task<IResponse> GetAllBywaitingForApprovalUserList(List_UserApproval model, int diff);
        Task<IResponse> GetAllAgainRequestsForCourseApproval(List_UserApproval model, int diff);
        Task<IResponse> ApproveUserRequest(Guid model, Guid userId);
        Task<IResponse> AddEditEnrollment(AddEditEnrollment model, Guid userId);
        Task<IResponse> GetById(Guid Id);
        Task<IResponse> GetAllCoursesByCategoryId(Guid Id);
        Task<IResponse> updateEnrollmentDisableStatus(ActiveInactive model);

    }
}