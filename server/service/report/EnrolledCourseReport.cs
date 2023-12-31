using System.Threading.Tasks;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Paradigm.Server.Application
{
    public class EnrolledCourseReport : IEnrolledCourseReport
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICountResponse _countResp;
        public EnrolledCourseReport(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }

        public async Task<IResponse> GetEnrolledUserReport(EnrolledCourse model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "UserName" : model.Sort;
            DateTime currentDate = DateTime.UtcNow;
            //LINQ
            var data = (from enrollment in _dbContext.Enrollment
                        join userDetail in _dbContext.UserDetail on enrollment.UserId equals userDetail.UserId
                        join user in _dbContext.User on enrollment.UserId equals user.UserId
                        join course in _dbContext.Course on enrollment.CourseId equals course.CourseId
                        where (
                        (EF.Functions.ILike(enrollment.UserName, $"%{model.UserName}%") || String.IsNullOrEmpty(model.UserName)) &&
                        (EF.Functions.ILike(userDetail.CNIC, $"%{model.CNIC}%") || String.IsNullOrEmpty(model.CNIC)) &&
                        (EF.Functions.ILike(course.Title, $"%{model.CourseTitle}%") || String.IsNullOrEmpty(model.CourseTitle)) &&
                        (enrollment.EnrolledStatus == model.EnrolledStatus || String.IsNullOrEmpty(model.EnrolledStatus))
                        )
                        select new VW_EnrolledCourseReport
                        {
                            EnrollmentId=enrollment.EnrolledId,
                            CourseTitle = course.Title,
                            UserName = enrollment.UserName,
                            CNIC = userDetail.CNIC,
                            EnrolledStatus = enrollment.EnrolledStatus,
                            MobileNumber = user.MobileNumber,
                            Status=enrollment.Disable,
                            TimeRestriction=DateTime.Compare(currentDate, enrollment.TimeRestriction)
                        }).AsQueryable();

            //Sort and Return
            var count = data.Count();
            var sorted = await HelperStatic.OrderBy(data, model.SortEx, model.OrderEx == "desc").Skip(model.Start).Take(model.LimitEx).ToListAsync();
            foreach (var item in sorted)
            {
                item.TotalCount = count;
                item.SerialNo = ++model.Start;
            }
            _countResp.DataList = sorted;
            _countResp.TotalCount = sorted.Count > 0 ? sorted.First().TotalCount : 0;
            _response.Success = Constants.ResponseSuccess;
            _response.Data = _countResp;
            return _response;

        }
        public async Task<IResponse> GetEnrolledCourseCount(EnrolledCourseCount model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "CourseTitle" : model.Sort;
            model.CourseTitle = String.IsNullOrEmpty(model.CourseTitle) ? "" : model.CourseTitle;
            var data = await _dbContext.Course
            .Where(
            (x => x.Title.ToLower().Contains(model.CourseTitle.ToLower()) || String.IsNullOrEmpty(model.CourseTitle))
            )
           .Select(x => new CourseConunt
           {
               CourseName = x.Title,
               Count = _dbContext.Enrollment.Count(y => y.CourseId == x.CourseId)
           })
           .OrderByDescending(x => x.Count) // Sort by count in descending order
           .Skip(model.Start)
           .Take(model.LimitEx)
           .ToListAsync();

            var count = await _dbContext.Course.CountAsync();
            foreach (var item in data)
            {
                item.TotalCount = count;
                item.SerialNo = ++model.Start;
            }
            _countResp.DataList = data;
            _countResp.TotalCount = count > 0 ? data.First().TotalCount : 0;
            _response.Success = Constants.ResponseSuccess;
            _response.Data = _countResp;
            return _response;
        }
    }
}
