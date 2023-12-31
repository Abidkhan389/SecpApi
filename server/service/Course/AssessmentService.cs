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
    public class AssessmentService : IAssessmentService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        private readonly ICountResponse _countResp;

        public AssessmentService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            this._dbContext = dbContext;
            this._response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetAllByProc(List_Assessment model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "Question" : model.Sort;
            //LINQ
            var data = (from assessment in _dbContext.Assessment
                        join createdBy in _dbContext.User on assessment.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on assessment.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        join course in _dbContext.Course on assessment.CourseId equals course.CourseId
                        where (
                         (EF.Functions.ILike(assessment.Question, $"%{model.Question}%") || String.IsNullOrEmpty(model.Question))
                         && (assessment.Status == model.Status || model.Status == null)
                         && (assessment.DifficultyLevel == model.DifficultyType || model.DifficultyType == null)
                         && (assessment.QuestionType == model.QuestionType || model.QuestionType == null)
                         && (assessment.CourseId == model.CourseId || model.CourseId == null))
                        select new VW_Assessment
                        {
                            AssessmentId = assessment.AssessmentId,
                            Question = assessment.Question,
                            Status = assessment.Status,
                            CourseName = course.Title,
                            QuestionType = assessment.QuestionType,
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
        public async Task<IResponse> AddEditAssessment(AddEditAssessment model, Guid UserId)
        {
            if (model.AssessmentId == null)
            {
                var question = await _dbContext.Assessment.FirstOrDefaultAsync(x => x.Question == model.Question);
                if (question != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "Question");
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }
                Assessment assessment = new Assessment(model, UserId, HelperStatic.GetCurrentTimeStamp());
                await _dbContext.Assessment.AddAsync(assessment);
                await _dbContext.SaveChangesAsync();
                _response.Data = assessment;
                _response.Success = Constants.ResponseSuccess;
                _response.Message = Constants.DataSaved;
                return _response;
            }
            else
            {
                var editAssessment = await _dbContext.Assessment.FirstOrDefaultAsync(x => x.AssessmentId == model.AssessmentId);
                if (editAssessment != null)
                {
                    editAssessment.Question = model.Question;
                    editAssessment.Options = JsonSerializer.Serialize(model.Options);
                    editAssessment.CourseId = model.CourseId;
                    editAssessment.UpdatedBy = UserId;
                    editAssessment.QuestionType = model.QuestionType;
                    editAssessment.DifficultyLevel = model.DifficultyLevel;
                    editAssessment.Medium = Convert.ToInt32(model.Medium);
                    editAssessment.LanguageTypes = model.LanguageTypes;
                    editAssessment.UpdatedOn = HelperStatic.GetCurrentTimeStamp();
                    editAssessment.Explanation = model.Explanation;
                    _dbContext.Assessment.Update(editAssessment);
                    await _dbContext.SaveChangesAsync();
                    _response.Success = Constants.ResponseSuccess;
                    _response.Message = Constants.DataUpdate;
                    return _response;
                }
                _response.Message = Constants.NotFound.Replace("{data}", "Assessment Question");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
        }
        public async Task<IResponse> GetAssessmentQuestionById(Guid Id)
        {
            var assessment = await _dbContext.Assessment.FirstOrDefaultAsync(x => x.AssessmentId == Id);
            if (assessment == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Assessment Question");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            AddEditAssessment objasssessment = new();
            objasssessment.AssessmentId = assessment.AssessmentId;
            objasssessment.Question = assessment.Question;
            objasssessment.CourseId = assessment.CourseId;
            objasssessment.Status = assessment.Status;
            objasssessment.Explanation = assessment.Explanation;
            objasssessment.DifficultyLevel = assessment.DifficultyLevel;
            objasssessment.QuestionType = assessment.QuestionType;
            objasssessment.Medium = assessment.Medium;
            objasssessment.LanguageTypes = assessment.LanguageTypes;
            objasssessment.Options = JsonSerializer.Deserialize<List<OptionsOfQuestions>>(assessment.Options);
            _response.Data = objasssessment;
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }
        public async Task<IResponse> ActiveInactive(ActiveInactive model)
        {
            var assessment = await _dbContext.Assessment.FirstOrDefaultAsync(x => x.AssessmentId == model.Id);
            if (assessment == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Assessment Question");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            assessment.Status = model.Status;
            _dbContext.Assessment.Update(assessment);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetAssessmentQuestions(Guid Id)
        {
            var assessmentQuestions = await (from assessment in _dbContext.Assessment
                                             join course in _dbContext.Course on assessment.CourseId equals course.CourseId
                                             where (
                                                assessment.CourseId == Id && assessment.Status == 1
                                                )
                                             select new VW_AssessmentQuestions
                                             {
                                                 AssessmentId = assessment.AssessmentId,
                                                 Question = assessment.Question,
                                                 LanguageTypes = assessment.LanguageTypes,
                                                 LinkId = false,
                                                 Explanation = assessment.Explanation,
                                                 CourseName = course.Title,
                                                 Options = JsonSerializer.Deserialize<List<OptionsOfQuestions>>(assessment.Options, new JsonSerializerOptions()),
                                             }).ToListAsync();
            foreach (var item in assessmentQuestions)
            {
                item.AnswerArr = item.Options.Select(o => new AssessmentAnswers
                {
                    Answer = o.OptionTitle,
                    IsChecked = false
                }).ToList();
            }
            if (assessmentQuestions == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Questions");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = assessmentQuestions;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }

    }
}
