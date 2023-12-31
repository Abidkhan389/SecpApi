using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using static Paradigm.Data.Model.Lecture;

namespace Paradigm.Server.Application
{

    public class LectureService : ILetureService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICountResponse _countResp;
        public LectureService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetAllByProc(List_Lectures model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "LectureTitle" : model.Sort;
            //LINQ
            var data = (from lecture in _dbContext.Lecture
                        join course in _dbContext.Course on lecture.CourseId equals course.CourseId
                        join createdBy in _dbContext.User on lecture.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on lecture.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        where (
                        (EF.Functions.ILike(lecture.LectureTitle, $"%{model.LectureTitle}%") || String.IsNullOrEmpty(model.LectureTitle))
                          && (lecture.Status == model.Status || model.Status == null) &&
                          (lecture.CourseId == model.CourseId || model.CourseId == null))
                        select new VW_Lecture
                        {
                            LectureTitle = lecture.LectureTitle,
                            CourseId = lecture.CourseId,
                            CourseName = course.Title,
                            LectureId = lecture.LectureId,
                            LectureNumber = lecture.LectureNumber,
                            Description = lecture.Description,
                            Status = lecture.Status,
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
        public async Task<IResponse> GetLecturesBycourse(Guid Id)
        {
            var lectureList = await _dbContext.Lecture.Where(x => x.CourseId == Id).ToListAsync();
            if (lectureList == null)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = lectureList;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetLectureById(Guid Id)
        {
            var lectureObj = await _dbContext.Lecture.FirstOrDefaultAsync(x => x.LectureId == Id);
            if (lectureObj == null)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = lectureObj;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> AddEditLecture(AddEditLecture model, Guid userId)
        {
            if (model.LectureId == null)
            {
                var lectureObj = await _dbContext.Lecture.FirstOrDefaultAsync(x => x.LectureNumber == model.LectureNumber && x.CourseId == model.CourseId);
                if (lectureObj != null)
                {
                    _response.Message = ("Lecture Already exists");
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }
                Lecture lecture = new Lecture(model, userId, HelperStatic.GetCurrentTimeStamp());

                await _dbContext.Lecture.AddAsync(lecture);
                await _dbContext.SaveChangesAsync();
                _response.Data = lecture;
                _response.Success = Constants.ResponseSuccess;
                _response.Message = Constants.DataSaved;
                return _response;
            }
            else
            {
                var editLecture = await _dbContext.Lecture.FirstOrDefaultAsync(x => x.LectureId == model.LectureId);
                if (editLecture != null)
                {
                    editLecture.LectureNumber = model.LectureNumber;
                    editLecture.Description = model.Description;
                    editLecture.LectureNumber = model.LectureNumber;
                    editLecture.LectureTitle = model.LectureTitle;
                    editLecture.CourseId = model.CourseId;
                    editLecture.UpdatedBy = userId;
                    editLecture.UpdatedOn = HelperStatic.GetCurrentTimeStamp();
                    _dbContext.Lecture.Update(editLecture);
                    await _dbContext.SaveChangesAsync();
                    _response.Success = Constants.ResponseSuccess;
                    _response.Message = Constants.DataUpdate;
                    return _response;
                }
                _response.Message = Constants.NotFound.Replace("{data}", "VideoTraining");
                _response.Success = Constants.ResponseFailure;
                return _response;

            }
        }
        public async Task<IResponse> ActiveInactive(ActiveInactive model)
        {
            Lecture lecture = await _dbContext.Lecture.FirstOrDefaultAsync(x => x.LectureId == model.Id);
            if (lecture == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Lecture");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            lecture.Status = model.Status;
            _dbContext.Lecture.Update(lecture);
            var courseContent = await _dbContext.CourseContent.Where(x => x.LectureId == lecture.LectureId).ToListAsync();
            foreach (var content in courseContent)
            {
                content.Status = model.Status;
            }
            _dbContext.CourseContent.UpdateRange(courseContent);
            var quiz = await _dbContext.Quiz.Where(x => x.LectureId == lecture.LectureId).ToListAsync();
            foreach (var question in quiz)
            {
                question.Status = model.Status;
            }
            _dbContext.Quiz.UpdateRange(quiz);

            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetLectureWithContent(Guid Id, Guid UserId)
        {
            VW_LectureWithContent lectureObj = new VW_LectureWithContent();
            lectureObj = await (from lecture in _dbContext.Lecture
                                join course in _dbContext.Course on lecture.CourseId equals course.CourseId
                                //join tempGrade in _dbContext.TempGrade on lecture.LectureId equals tempGrade.LectureId
                                where
                                   (
                                    lecture.LectureId == Id
                                   )
                                select new VW_LectureWithContent
                                {
                                    CourseId = lecture.CourseId,
                                    CourseName = course.Title,
                                    LectureTitle = lecture.LectureTitle,
                                    LectureNumber = lecture.LectureNumber,
                                    Description = lecture.Description,
                                    // isattempt = true

                                }).FirstOrDefaultAsync();
            var CheckQuizAlreadyAttempted = await _dbContext.TempGrade.FirstOrDefaultAsync(x => x.LectureId == Id && x.UserId == UserId);
            if (CheckQuizAlreadyAttempted != null)
            {
                lectureObj.isattempt = true;
            }
            var content = await _dbContext.CourseContent.Where(y => y.LectureId == Id && y.Status == 1).ToListAsync();
            lectureObj.CourseContents = content;
            if (lectureObj == null)
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
