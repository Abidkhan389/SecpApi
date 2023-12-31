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
    public class CompletedCoursesGradeeport : ICompletedCoursesGradeReport
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICountResponse _countResp;
        public CompletedCoursesGradeeport(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }

        public async Task<IResponse> GetAllUserGrades(CompletedCourseGrade model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "CourseTitle" : model.Sort;
            //LINQ
            var data = (from grade in _dbContext.UserGrade
                        join enrollment in _dbContext.Enrollment on grade.UserId equals enrollment.UserId
                        join course in _dbContext.Course on grade.CourseId equals course.CourseId
                        where (
                        (EF.Functions.ILike(enrollment.UserName, $"%{model.UserName}%") || String.IsNullOrEmpty(model.UserName)) &&
                        (EF.Functions.ILike(course.Title, $"%{model.CourseTitle}%") || String.IsNullOrEmpty(model.CourseTitle)) &&
                        (grade.Grade == model.Grade || String.IsNullOrEmpty(model.Grade)) &&
                           enrollment.EnrolledStatus == "1"
                          )
                        group new { grade, course, enrollment } by new { grade.UserId, grade.CourseId, course.Title } into grp
                        select new VW_CompletedCourseGrade
                        {
                            GradeId = grp.First().grade.GradeId,
                            CourseTitle = grp.Key.Title,
                            UserName = grp.First().enrollment.UserName,
                            Grade = grp.First().grade.Grade
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
        public async Task<IResponse> GetGradeById(Guid id)
        {
            var grade = await (from userGrade in _dbContext.UserGrade
                               join user in _dbContext.User on userGrade.UserId equals user.UserId
                               join course in _dbContext.Course on userGrade.CourseId equals course.CourseId
                               where (
                                  userGrade.GradeId == id)
                               select new VW_CompletedCourseGrade
                               {
                                   CourseTitle = course.Title,
                                   UserName = user.Username,
                                   Grade = userGrade.Grade,
                                   Date = userGrade.Date,
                               }).FirstOrDefaultAsync();
            _response.Data = grade;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }

    }
}
