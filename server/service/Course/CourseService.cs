using System.Threading.Tasks;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Paradigm.Server.Application
{
    public class CourseService : ICourseService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICountResponse _countResp;

        public CourseService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetAllByProc(List_Courses model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "Title" : model.Sort;
            //LINQ
            var data = (from course in _dbContext.Course
                        join createdBy in _dbContext.User on course.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on course.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        join Category in _dbContext.Category on course.CategoryId equals Category.CategoryId
                        where (
                         (EF.Functions.ILike(course.Title, $"%{model.Title}%") || String.IsNullOrEmpty(model.Title))
                         && (course.Status == model.Status || model.Status == null))
                        select new VW_Course
                        {
                            CourseId = course.CourseId,
                            Title = course.Title,
                            Description = course.Description,
                            Icon = course.Icon,
                            Value = course.Value,
                            Status = course.Status,
                            CategoryName = Category.CategoryName
                            // CreatedBy = updateBy == null ? updatedUserr.DisplayName : updateBy.DisplayName,
                            // CreatedOn = (int)(Category.UpdatedOn == null ? Category.CreatedOn + diff : Category.UpdatedOn.Value + diff)
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
        public async Task<IResponse> AddEditCourse(VW_Courseaddedit model, Guid userId)
        {
            if (model.CourseId == null)
            {
                var courseObj = await _dbContext.Course.FirstOrDefaultAsync(x => x.Title.ToLower() == model.Title.ToLower());
                if (courseObj != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "Course");
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }
                Course course = new Course(model, userId, HelperStatic.GetCurrentTimeStamp());
                await _dbContext.Course.AddAsync(course);
                await _dbContext.SaveChangesAsync();
                _response.Data = course;
                _response.Success = Constants.ResponseSuccess;
                _response.Message = Constants.DataSaved;
                return _response;
            }
            else
            {
                var editCourse = await _dbContext.Course.FirstOrDefaultAsync(x => x.CourseId == model.CourseId);
                if (editCourse != null)
                {
                    editCourse.Title=model.Title;
                    if(editCourse.Title.ToLower() == model.Title.ToLower())
                    {
                        _response.Message = Constants.Exists.Replace("{data}", "Course");
                        _response.Success = Constants.ResponseFailure;
                        return _response;
                    }
                    editCourse.Description = model.Description;
                    editCourse.Icon = model.Icon;
                    editCourse.Value = model.Value;
                    editCourse.UpdatedBy = userId;
                    editCourse.UpdatedOn = HelperStatic.GetCurrentTimeStamp();
                    editCourse.CategoryId = model.CategoryId;
                    editCourse.Duration = model.Duration;
                    _dbContext.Course.Update(editCourse);
                    await _dbContext.SaveChangesAsync();
                    _response.Success = Constants.ResponseSuccess;
                    _response.Message = Constants.DataUpdate;
                    return _response;
                }
                _response.Message = Constants.NotFound.Replace("{data}", "Course");
                _response.Success = Constants.ResponseFailure;
                return _response;

            }

        }
        public async Task<IResponse> GetCourseById(Guid Id)
        {
            VW_Courseaddedit objCourse = new VW_Courseaddedit();
            objCourse = await (from course in _dbContext.Course
                               join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                               where (course.CourseId == Id)
                               select new VW_Courseaddedit
                               {
                                   CourseId = course.CourseId,
                                   Title = course.Title,
                                   Description = course.Description,
                                   Icon = course.Icon,
                                   Value = course.Value,
                                   Status = course.Status,
                                   CategoryId = category.CategoryId,
                                   Duration = course.Duration,
                               }).FirstOrDefaultAsync();

            //var trainingList = await _dbContext.Trainings.Where(x => x.CourseId == courseObj.CourseId).ToListAsync();
            // objCourse.TrainingsList = trainingList;

            _response.Data = objCourse;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> ActiveInactive(ActiveInactive model)
        {
            Course course = await _dbContext.Course.FirstOrDefaultAsync(x => x.CourseId == model.Id);
            if (course == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Category");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            course.Status = model.Status;
            _dbContext.Course.Update(course);
            var lecture = await _dbContext.Lecture.Where(x => x.CourseId == course.CourseId).ToListAsync();
            foreach (var item in lecture)
            {
                item.Status = model.Status;
            }
            _dbContext.Lecture.UpdateRange(lecture);
            var courseContent = await _dbContext.CourseContent.Where(x => x.CourseId == course.CourseId).ToListAsync();
            foreach (var content in courseContent)
            {
                content.Status = model.Status;
            }
            _dbContext.CourseContent.UpdateRange(courseContent);
            var quiz = await _dbContext.Quiz.Where(x => x.CourseId == course.CourseId).ToListAsync();
            foreach (var question in quiz)
            {
                question.Status = model.Status;
            }
            _dbContext.Quiz.UpdateRange(quiz);


            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }
        public async Task<IResponse> GetAllCourses(Guid UserId)
        {
            var courseList = await (from course in _dbContext.Course
                                    join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                                    join enrollment in _dbContext.Enrollment.Where(e => e.UserId == UserId && (e.EnrolledStatus == "0" || e.EnrolledStatus == "1")) on course.CourseId equals enrollment.CourseId into courseEnrollment
                                    from ce in courseEnrollment.DefaultIfEmpty()
                                    join tempEnrollment in _dbContext.TempEnrollment.Where(t => t.UserId == UserId && t.Status == 0) on course.CourseId equals tempEnrollment.CourseId into courseTempEnrollment
                                    from cte in courseTempEnrollment.DefaultIfEmpty()
                                    where (
                                        ce != null || cte != null || !_dbContext.Enrollment.Any(e => e.CourseId == course.CourseId && e.UserId == UserId))
                                    select new VW_ListOfAllCourses
                                    {
                                        CourseId = course.CourseId,
                                        Title = course.Title,
                                        Icon = course.Icon,
                                        CategoryName = category.CategoryName,
                                        CategoryId = course.CategoryId,
                                        Description = course.Description,
                                        EnrolledStatus = ce == null ? "" : ce.EnrolledStatus,
                                        AppliedStatus = cte == null ? "0" : "1"
                                    }).ToListAsync();

            if (courseList == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Courses");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }

            _response.Success = Constants.ResponseSuccess;
            _response.Data = courseList;
            return _response;
        }
        //Get Course Details for user with lectureList
        public async Task<IResponse> GetCoursewithLectures(Guid Id, Guid UserId)
        {
            var objCourse = await (from course in _dbContext.Course
                                   join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                                   join enrollment in _dbContext.Enrollment on course.CourseId equals enrollment.CourseId
                                   where (
                                    course.CourseId == Id
                                   && enrollment.EnrolledStatus == "0")
                                   select new VW_CourseForUser
                                   {
                                       CourseId = course.CourseId,
                                       Title = course.Title,
                                       Description = course.Description,
                                       Icon = course.Icon,
                                       Value = course.Value,
                                       CategoryName = category.CategoryName,
                                       //AssessmentAttempt = false,
                                   }).FirstOrDefaultAsync();

            var lectures = await _dbContext.Lecture.Where(x => x.CourseId == objCourse.CourseId && x.Status == 1).ToListAsync();
            objCourse.LectureList = lectures;
            // var asessmentCheck = _dbContext.TempGrade.Where(x => x.CourseId == Id && x.GradeTemp != "F" && x.UserId == UserId).Count();
            // var totalLectures = _dbContext.Lecture.Where(x => x.CourseId == Id && x.Status == 1).Count();
            // if (asessmentCheck == totalLectures)
            // {
            //     objCourse.AssessmentAttempt = true;
            // }

            _response.Data = objCourse;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetAllCategoriesForCourses()
        {
            var courseObj = await _dbContext.Category.Select(x => new VW_CategoryForCoursed { CategoryName = x.CategoryName, CategoryId = x.CategoryId }).ToListAsync();
            if (courseObj == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Category");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = courseObj;
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }

        public async Task<IResponse> GetCoursesByCategoryId(Guid CategoryId, Guid UserId)
        {
            var courseList = await (from course in _dbContext.Course
                                    join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                                    join enrollment in _dbContext.Enrollment.Where(e => e.UserId == UserId && (e.EnrolledStatus == "0" || e.EnrolledStatus == "1")) on course.CourseId equals enrollment.CourseId into courseEnrollment
                                    from ce in courseEnrollment.DefaultIfEmpty()
                                    join tempEnrollment in _dbContext.TempEnrollment.Where(t => t.UserId == UserId && t.Status == 0) on course.CourseId equals tempEnrollment.CourseId into courseTempEnrollment
                                    from cte in courseTempEnrollment.DefaultIfEmpty()
                                    where (
                                        (course.Status == 1) && (category.CategoryId == CategoryId) &&
                                        (ce != null || cte != null || !_dbContext.Enrollment.Any(e => e.CourseId == course.CourseId && e.UserId == UserId)))
                                    select new VW_ListOfAllCourses
                                    {
                                        CourseId = course.CourseId,
                                        Title = course.Title,
                                        Icon = course.Icon,
                                        CategoryName = category.CategoryName,
                                        CategoryId = course.CategoryId,
                                        Description = course.Description,
                                        EnrolledStatus = ce == null ? "" : ce.EnrolledStatus,
                                        AppliedStatus = cte == null ? "0" : "1"
                                    }).ToListAsync();

            if (courseList.Count <= 0)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Courses");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }

            _response.Data = courseList;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }

        //Get Courses in which user Enrolled
        public async Task<IResponse> GetEnrolledCourses(Guid UserId)
        {
            DateTime currentDate = DateTime.UtcNow;
            var courseList = await (from course in _dbContext.Course
                                    join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                                    join enrollment in _dbContext.Enrollment on course.CourseId equals enrollment.CourseId
                                    where (
                                     course.Status == 1 && enrollment.UserId == UserId && enrollment.EnrolledStatus == "0")
                                    select new EnrolledCourses
                                    {
                                        CategoryName = category.CategoryName,
                                        CourseId = course.CourseId,
                                        Title = course.Title,
                                        Description = course.Description,
                                        Icon = course.Icon,
                                        Value = course.Value,
                                        TimeRestriction = DateTime.Compare(currentDate, enrollment.TimeRestriction),
                                        Disable = enrollment.Disable,
                                    }).ToListAsync();
            if (courseList == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Courses");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Success = Constants.ResponseSuccess;
            _response.Data = courseList;
            return _response;

        }
        public async Task<IResponse> GetEnrolledCoursesByCategoryId(Guid CategoryId, Guid UserId)
        {
            DateTime currentDate = DateTime.UtcNow;
            var courseList = await (from course in _dbContext.Course
                                    join Category in _dbContext.Category on course.CategoryId equals Category.CategoryId
                                    join enrollment in _dbContext.Enrollment on course.CourseId equals enrollment.CourseId
                                    where (
                                        course.CategoryId == CategoryId && enrollment.UserId == UserId && enrollment.EnrolledStatus == "0")
                                    select new EnrolledCourses
                                    {
                                        Title = course.Title,
                                        CourseId = course.CourseId,
                                        Icon = course.Icon,
                                        Value = course.Value,
                                        CategoryName = Category.CategoryName,
                                        TimeRestriction = DateTime.Compare(currentDate, enrollment.TimeRestriction),
                                        Disable = enrollment.Disable,
                                    }).ToListAsync();
            if (courseList.Count <= 0)
            {

                _response.Message = ("You are not enrolled for any course of the selected Category");
                _response.Success = Constants.ResponseFailure;
                return _response;

            }
            _response.Data = courseList;
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }

    }

}
