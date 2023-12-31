using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface IEnrolledCourseReport
    {
        Task<IResponse> GetEnrolledUserReport(EnrolledCourse model, int diff);
         Task<IResponse> GetEnrolledCourseCount(EnrolledCourseCount model, int diff);

    }
}