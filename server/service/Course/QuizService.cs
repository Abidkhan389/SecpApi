using System.Threading.Tasks;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text.Json;

namespace Paradigm.Server.Application
{
    public class QuizService : IQuizService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICountResponse _countResp;

        public QuizService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetAllCourses()
        {
            var courseObj = await _dbContext.Course.Select(x => new CourseList { Title = x.Title, CourseId = x.CourseId }).ToListAsync();
            if (courseObj == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "course");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = courseObj;
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }
        public async Task<IResponse> GetAllLectures(Guid Id)
        {
            var LectureList = await _dbContext.Lecture.Where(x => x.CourseId == Id).Select(x => new Lecture { LectureTitle = x.LectureTitle, LectureId = x.LectureId }).ToListAsync();
            if (LectureList.Count <= 0)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Lectures");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = LectureList;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }


        public async Task<IResponse> GetAllByProc(List_Quiz model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "Question" : model.Sort;
            //LINQ
            var data = (from quiz in _dbContext.Quiz
                        join createdBy in _dbContext.User on quiz.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on quiz.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        join course in _dbContext.Course on quiz.CourseId equals course.CourseId
                        join lec in _dbContext.Lecture on quiz.LectureId equals lec.LectureId
                        where (
                         (EF.Functions.ILike(quiz.Question, $"%{model.Question}%") || String.IsNullOrEmpty(model.Question))
                         && (quiz.Status == model.Status || model.Status == null)
                         && (quiz.DifficultyLevel == model.DifficultyType || model.DifficultyType == null)
                         && (quiz.Type == model.QuestionType || model.QuestionType == null)
                         && (quiz.CourseId == model.CourseId || model.CourseId == null)
                         && (quiz.LectureId == model.LectureId || model.LectureId == null))
                        select new VW_quiz
                        {
                            QuestionId = quiz.QuestionId,
                            Question = quiz.Question,
                            Status = quiz.Status,
                            CourseName = course.Title,
                            Type = quiz.Type,
                            LectureName = lec.LectureTitle
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
        public async Task<IResponse> AddEditQuestion(VW_Quizaddedit model, Guid UserId)
        {
            if (model.QuestionId == null)
            {
                var courseObj = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Question == model.Question);
                if (courseObj != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "Question");
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }
                Quiz quiz = new Quiz(model, UserId, HelperStatic.GetCurrentTimeStamp());
                await _dbContext.Quiz.AddAsync(quiz);
                await _dbContext.SaveChangesAsync();
                _response.Data = quiz;
                _response.Success = Constants.ResponseSuccess;
                _response.Message = Constants.DataSaved;
                return _response;
            }
            else
            {
                var editquiz = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.QuestionId == model.QuestionId);
                if (editquiz != null)
                {
                    editquiz.Question = model.Question;
                    editquiz.Options = JsonSerializer.Serialize(model.Options);
                    editquiz.CourseId = model.CourseId;
                    editquiz.UpdatedBy = UserId;
                    editquiz.Type = model.Type;
                    editquiz.DifficultyLevel = model.DifficultyLevel;
                    editquiz.LectureId = model.LectureId;
                    editquiz.Medium = Convert.ToInt32(model.Medium);
                    editquiz.LanguageTypes = model.LanguageTypes;
                    editquiz.UpdatedOn = HelperStatic.GetCurrentTimeStamp();
                    editquiz.Explanation = model.Explanation;
                    _dbContext.Quiz.Update(editquiz);
                    await _dbContext.SaveChangesAsync();
                    _response.Success = Constants.ResponseSuccess;
                    _response.Message = Constants.DataUpdate;
                    return _response;
                }
                _response.Message = Constants.NotFound.Replace("{data}", "Question");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
        }
        public async Task<IResponse> GetQuestionById(Guid Id)
        {
            Quiz quiz = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.QuestionId == Id);
            if (quiz == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "QuestionBank");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            VW_Quizaddedit objQuiz = new VW_Quizaddedit();
            objQuiz.QuestionId = quiz.QuestionId;
            objQuiz.Question = quiz.Question;
            objQuiz.CourseId = quiz.CourseId;
            objQuiz.Status = quiz.Status;
            objQuiz.Explanation = quiz.Explanation;
            objQuiz.DifficultyLevel = quiz.DifficultyLevel;
            objQuiz.Type = quiz.Type;
            objQuiz.Medium = quiz.Medium;
            objQuiz.LectureId = quiz.LectureId;
            objQuiz.LanguageTypes = quiz.LanguageTypes;
            objQuiz.Options = JsonSerializer.Deserialize<List<QuestionOptions>>(quiz.Options);
            _response.Data = objQuiz;
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }
        public async Task<IResponse> GetQuestionsByLecture(Guid courseId, Guid lectureId)
        {
            var questions = await (from quiz in _dbContext.Quiz
                                   join course in _dbContext.Course on quiz.CourseId equals course.CourseId
                                   join lec in _dbContext.Lecture on quiz.LectureId equals lec.LectureId
                                   where (
                                      quiz.CourseId == courseId && quiz.LectureId == lectureId && quiz.Status == 1
                                      )
                                   select new VW_Questions
                                   {
                                       QuestionId = quiz.QuestionId,
                                       Question = quiz.Question,
                                       LanguageTypes = quiz.LanguageTypes,
                                       LinkId = false,
                                       Explanation = quiz.Explanation,
                                       LectureName = lec.LectureTitle,
                                       CourseName = course.Title,
                                       LectureNumber = lec.LectureNumber,
                                       Options = JsonSerializer.Deserialize<List<QuestionOptions>>(quiz.Options, new JsonSerializerOptions()),
                                   }).ToListAsync();
            foreach (var item in questions)
            {
                item.AnswerArr = item.Options.Select(o => new Answers
                {
                    Answer = o.OptionTitle,
                    IsChecked = false
                }).ToList();
            }
            if (questions == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Questions");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = questions;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> ActiveInactive(ActiveInactive model)
        {
            Quiz quiz = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.QuestionId == model.Id);
            if (quiz == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Question");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            quiz.Status = model.Status;
            _dbContext.Quiz.Update(quiz);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }

    }
}
