using System;
using System.Threading.Tasks;
using Paradigm.Server.Application;
using Paradigm.Data.Model;

namespace Paradigm.Server.Interface
{
    public interface IGradeService
    {
        Task<IResponse> GetLectureWiseGrade(QuizDetail model);
        Task<IResponse> GetCourseWiseGrade(AssessmentDetail model);
        Task<IResponse> GetUserGrades(Guid UserId);
        Task<IResponse> GetUserGradeForCourse(Guid CourseId,Guid UserId);
    }
}