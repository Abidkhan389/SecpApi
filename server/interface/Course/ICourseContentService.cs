using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface ICourseContentService
    {
        Task<IResponse> GetAllByProc(List_Content model, int diff);
        Task<IResponse> ActiveInactive(ActiveInactive model);
        Task<IResponse> GetAllCoursesForTraining();
        Task<IResponse> GetCourseContentById(Guid Id);
        Task<IResponse> GetLecturesForCourse(Guid Id);
        Task<IResponse> GetContentforCourse(Guid Id);
        Task<IResponse> AddEditCourseContent(AddEditContent model, Guid UserId);
    }
}