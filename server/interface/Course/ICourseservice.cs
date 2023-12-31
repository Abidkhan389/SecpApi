using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface ICourseService
    {
        Task<IResponse> GetCourseById(Guid Id);
        Task<IResponse> GetEnrolledCoursesByCategoryId(Guid CategoryId, Guid UserId);
        Task<IResponse> GetCoursesByCategoryId(Guid CategoryId, Guid UserId);
        Task<IResponse> GetCoursewithLectures(Guid Id, Guid UserId);
        Task<IResponse> GetAllCategoriesForCourses();
        Task<IResponse> GetAllByProc(List_Courses model, int diff);
        Task<IResponse> GetAllCourses(Guid UserId);
        Task<IResponse> AddEditCourse(VW_Courseaddedit model, Guid userId);
        Task<IResponse> ActiveInactive(ActiveInactive model);
        Task<IResponse> GetEnrolledCourses(Guid UserId);


    }
}