using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface ICompletedCoursesGradeReport
    {
        Task<IResponse> GetAllUserGrades(CompletedCourseGrade model, int diff);
        Task<IResponse> GetGradeById(Guid Id);

    }
}