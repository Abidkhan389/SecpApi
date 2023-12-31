using System.Threading.Tasks;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Paradigm.Server.Application
{
    public class CourseContentService : ICourseContentService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICountResponse _countResp;

        public CourseContentService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetAllByProc(List_Content model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "ContentName" : model.Sort;
            //LINQ
            var data = (from courseContent in _dbContext.CourseContent
                        join course in _dbContext.Course on courseContent.CourseId equals course.CourseId
                        join lecture in _dbContext.Lecture on courseContent.LectureId equals lecture.LectureId
                        join createdBy in _dbContext.User on courseContent.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on courseContent.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        where (
                        (EF.Functions.ILike(courseContent.ContentName, $"%{model.ContentName}%") || String.IsNullOrEmpty(model.ContentName))
                          && (courseContent.Status == model.Status || model.Status == null) &&
                          (courseContent.CourseId == model.CourseId || model.CourseId == null) &&
                          (courseContent.LectureId == model.LectureId || model.LectureId == null) &&
                          (courseContent.Order == model.Order || model.Order == null) &&
                          (courseContent.Type == model.Type || model.Type == null)
                          )
                        select new VW_CourseContent
                        {
                            CourseName = course.Title,
                            CourseId = courseContent.CourseId,
                            LectureId = courseContent.LectureId,
                            LectureTitle = lecture.LectureTitle,
                            ContentName = courseContent.ContentName,
                            ContentId = courseContent.ContentId,
                            Link = courseContent.Link,
                            Type = courseContent.Type,
                            Order = courseContent.Order,
                            Status = courseContent.Status,
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
        public async Task<IResponse> GetAllCoursesForTraining()
        {
            var courseObj = await _dbContext.Course.Select(x => new VW_CourseForTraining { CourseName = x.Title, CourseId = x.CourseId }).ToListAsync();
            if (courseObj == null)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = courseObj;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> ActiveInactive(ActiveInactive model)
        {
            CourseContent courseContent = await _dbContext.CourseContent.FirstOrDefaultAsync(x => x.ContentId == model.Id);
            if (courseContent == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Training");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            courseContent.Status = model.Status;
            _dbContext.CourseContent.Update(courseContent);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetCourseContentById(Guid Id)
        {
            var courseContentObj = await _dbContext.CourseContent.FirstOrDefaultAsync(x => x.ContentId == Id);
            if (courseContentObj == null)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            courseContentObj.Attachments = !String.IsNullOrEmpty(courseContentObj.Attachments) ? courseContentObj.Attachments.Replace("\\", "/") : null;
            _response.Data = courseContentObj;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetContentforCourse(Guid Id)
        {
            var obj = await _dbContext.CourseContent.Where(x => x.CourseId == Id).ToListAsync();
            if (obj.Count <= 0)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = obj;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> AddEditCourseContent(AddEditContent model, Guid userId)
        {
            if (model.ContentId == null)
            {
                var obj = await _dbContext.CourseContent.FirstOrDefaultAsync
                (x => x.CourseId == model.CourseId &&
                x.LectureId == model.LectureId && x.Order == model.Order);
                if (obj != null)
                {
                    _response.Message = "Content at this order exist try with another order number";
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }
                CourseContent courseContent = new CourseContent(model, userId, HelperStatic.GetCurrentTimeStamp());

                await _dbContext.CourseContent.AddAsync(courseContent);
                await _dbContext.SaveChangesAsync();
                _response.Data = courseContent;
                _response.Success = Constants.ResponseSuccess;
                _response.Message = Constants.DataSaved;
                return _response;
            }
            else
            {
                var editContent = await _dbContext.CourseContent.FirstOrDefaultAsync(x => x.ContentId == model.ContentId);
                if (editContent != null)
                {
                    editContent.ContentName = model.ContentName;
                    editContent.Text = model.Text;
                    editContent.Link = model.Link;
                    editContent.Attachments = model.Attachments;
                    editContent.LectureId = model.LectureId;
                    editContent.Order = model.Order;
                    editContent.CourseId = model.CourseId;
                    editContent.UpdatedBy = userId;
                    editContent.UpdatedOn = HelperStatic.GetCurrentTimeStamp();
                    _dbContext.CourseContent.Update(editContent);
                    await _dbContext.SaveChangesAsync();
                    _response.Success = Constants.ResponseSuccess;
                    _response.Message = Constants.DataUpdate;
                    return _response;
                }
                _response.Message = Constants.NotFound.Replace("{data}", "Course Content");
                _response.Success = Constants.ResponseFailure;
                return _response;

            }
        }
        public async Task<IResponse> GetLecturesForCourse(Guid Id)
        {
            var lectureObj = await _dbContext.Lecture.Where(x => x.CourseId == Id && x.Status == 1).Select(x => new VW_LectureForCourse { LectureTitle = x.LectureTitle, LectureId = x.LectureId }).ToListAsync();
            if (lectureObj.Count <= 0)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = lectureObj;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }


    }

}