using System.Threading.Tasks;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System.Globalization;

namespace Paradigm.Server.Application
{
    public class DashboardService : IDashboardService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        //private readonly MailSettings _mailSettings;
        private readonly ICountResponse _countResp;

        public DashboardService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetOverViewForAdminDashboard()
        {
            AdminDashboard adminDashboard = new();
            // All courses with title and course id
            var AllCourses = await _dbContext.Course
                            .Where(x => x.Status == 1)
                            .Select(x => new AdminDashboardCourses
                            {
                                CourseId = x.CourseId,
                                CourseName = x.Title
                            })
                            .ToListAsync();
            var CoursesCount = AllCourses.Count(); // All Courses Count
            var UseresCount = await _dbContext.UserDetail // All Users Count
                        .Where(userdetail => userdetail.Role == "User" && userdetail.Status == 1)
                        .CountAsync();

            // in one course have how many users ,so here i am couting for it
            var CourseperUserCounts = await _dbContext.Enrollment
           .GroupBy(e => e.CourseId)
           .Select(g => new CourseUserCount
           {
               CourseId = g.Key,
               EnrollmentCount = g.Count()
           })
           .ToListAsync();
            var courseIds = CourseperUserCounts.Select(u => u.CourseId).ToList();
            var coursenames = await _dbContext.Course
               .Where(u => courseIds.Contains(u.CourseId))
               .Select(u => new
               {
                   CourseId = u.CourseId,
                   Coursename = u.Title
               })
               .ToListAsync();
            // Merge the UserPerCourseCounts with the coursenames
            var mergedDataforcoursecount = CourseperUserCounts
                .Join(coursenames,
                    upcc => upcc.CourseId,
                    u => u.CourseId,
                    (upcc, u) => new CoursePerUserCountWithDetails
                    {
                        CourseId = u.CourseId,
                        CourseName = u.Coursename,
                        EnrollmentCount = upcc.EnrollmentCount
                    })
                .ToList();
            // User have how many courses ,so here i am couting for it
            var UserPerCourseCounts = await _dbContext.Enrollment
            .GroupBy(e => e.UserId)
            .Select(g => new UserCourseCount
            {
                UserId = g.Key,
                EnrollmentCount = g.Count()
            })
            .ToListAsync();
            var userIds = UserPerCourseCounts.Select(u => u.UserId).ToList();
            var usernames = await _dbContext.User
                .Where(u => userIds.Contains(u.UserId))
                .Select(u => new
                {
                    UserId = u.UserId,
                    Username = u.Username
                })
                .ToListAsync();

            // Merge the UserPerCourseCounts with the usernames
            var mergedData = UserPerCourseCounts
                .Join(usernames,
                    upcc => upcc.UserId,
                    u => u.UserId,
                    (upcc, u) => new UserCourseCountWithDetails
                    {
                        UserId = u.UserId,
                        UserName = u.Username,
                        EnrollmentCount = upcc.EnrollmentCount
                    })
                .ToList();

            // All completed Courses by All Users
            var Courses = await _dbContext.Enrollment.ToListAsync();//.Where(x=> x.EnrolledStatus=="1").CountAsync();
            var completedCourseCount = Courses.Count(enrollment => enrollment.EnrolledStatus == "1");
            var coursesaddedlasttowmonths= await _dbContext.Course.Where(x=> x.Status==1).ToListAsync();
            // count for course which are added since last 8 months
            int coursecount=0;
           coursecount= HelperStatic.CourseCountDateforLastTwoMonths(coursesaddedlasttowmonths);
            // count for course cerfiticate which are done since last 8 months
            var CourseCertificate =Courses.Where(x => x.EnrolledStatus == "1").ToList();
            int coursecertificatecount=0;
            coursecertificatecount=HelperStatic.CountDateforLastTwoMonths(CourseCertificate);
            // Count for useres which are created since last 8 months
            var users= await _dbContext.UserDetail.Where(x=> x.Status==1).ToListAsync();
            var currentDate = DateTime.UtcNow;
            var twoMonthsAgo = currentDate.AddMonths(-8);
            int lasttwomonthsuseraddedcount=0;
            foreach (var item in users)
            {
                // Assuming 'createdon' is a property of type int in your Enrollment class
                int? timestamp = item.CreatedOn; // Replace 'createdon' with the actual property name
                if (timestamp.HasValue)
                {
                    DateTime  courseDate = HelperStatic.TimestampToDate(timestamp);

                    if (courseDate >= twoMonthsAgo && courseDate <= currentDate)
                    {
                        lasttwomonthsuseraddedcount++;
                    }
                }
            }
            adminDashboard.LastTwoMonthsCourseCount=coursecount;
            adminDashboard.LastTwoMonthsUserCtreateCount=lasttwomonthsuseraddedcount;
            adminDashboard.LastTwoMonthsCompleteCourseCertificateCount=coursecertificatecount;
            adminDashboard.EnrolledUserCount = UseresCount;
            adminDashboard.CourseCount = CoursesCount;
            adminDashboard.CourseUserCountdetails = mergedDataforcoursecount;
            //adminDashboard.DailyBasedEnrolledUserCount=enrolledOnCurrentDateCount;
            adminDashboard.CompletedCoursesCount = completedCourseCount;
            adminDashboard.AllCourses = AllCourses;
            adminDashboard.UserPerCourseCount = mergedData;
            _response.Success = Constants.ResponseSuccess;
            _response.Data = adminDashboard;
            return _response;
        }
    }
}