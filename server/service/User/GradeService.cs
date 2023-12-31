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
    public class GradeService : IGradeService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;

        public GradeService(DbContextBase dbContext, IResponse response)
        {
            _dbContext = dbContext;
            _response = response;
        }

        public async Task<IResponse> GetLectureWiseGrade(QuizDetail model)
        {
            var tempGrade = await _dbContext.TempGrade.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.CourseId == model.CourseId && x.LectureId == model.LectureId && x.GradeTemp != "F");
            if (tempGrade == null)
            {
                int correctAnswer = 0;
                int totalNumberQuestions = _dbContext.Quiz.Count(x => x.CourseId == model.CourseId && x.LectureId == model.LectureId && x.Status == 1);
                var questions = await _dbContext.Quiz.Where(x => x.CourseId == model.CourseId && x.LectureId == model.LectureId && x.Status == 1).Select(x => new VW_Questions
                {
                    QuestionId = x.QuestionId,
                    Question = x.Question,
                    Options = JsonSerializer.Deserialize<List<QuestionOptions>>(x.Options, new JsonSerializerOptions()),
                }).ToListAsync();


                foreach (var item in questions)
                {
                    var answerObj = model.QuizAnswers.FirstOrDefault(x => x.QuestionId == item.QuestionId);
                    var answer = item.Options.FirstOrDefault(o => o.OptionNo == answerObj.SelectedOption && o.IsAnswer == true);
                    if (answer != null)
                    {
                        correctAnswer++;
                    }
                }

                //Percentage Calculation
                double percentage = (double)correctAnswer / (double)totalNumberQuestions * 100;
                string Grade = "";
                var gradeobj = await _dbContext.GradingCriteria.Where(x => x.Grade_S_R <= percentage && x.Grade_E_R >= percentage).FirstOrDefaultAsync();
                Grade = gradeobj.GradeName;
                // if (percentage >= 90)
                // {
                //     Grade = "A";
                // }
                // else if (percentage >= 80)
                // {
                //     Grade = "B";
                // }
                // else if (percentage >= 70)
                // {
                //     Grade = "C";
                // }
                // else if (percentage >= 60)
                // {
                //     Grade = "D";
                // }
                // else
                // {
                //     Grade = "F";
                // }
                var updateGrade = await _dbContext.TempGrade.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.CourseId == model.CourseId && x.LectureId == model.LectureId && x.GradeTemp == "F");
                if (updateGrade != null)
                {
                    updateGrade.GradeTemp = Grade;
                    _dbContext.TempGrade.Update(updateGrade);
                    await _dbContext.SaveChangesAsync();
                    _response.Data = updateGrade;
                    _response.Success = Constants.ResponseSuccess;
                    return _response;

                }
                else
                {

                    TempGrade tempgrade = new ()
                    {
                        TempGradeId =  Guid.NewGuid(),
                        UserId = model.UserId,
                        CourseId = model.CourseId,
                        LectureId = model.LectureId,
                        Percentage = percentage,
                        GradeTemp = Grade,
                    };
                    _dbContext.TempGrade.Add(tempgrade);
                    await _dbContext.SaveChangesAsync();
                    _response.Data = tempgrade;
                    _response.Success = Constants.ResponseSuccess;
                    return _response;
                }



            }
            _response.Message = ("You'r already Passed this quiz.");
            _response.Success = Constants.ResponseFailure;
            return _response;
        }
        public async Task<IResponse> GetCourseWiseGrade(AssessmentDetail model)
        {
            var grade = await _dbContext.UserGrade.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.CourseId == model.CourseId && x.Grade != "F");
            if (grade == null)
            {
                int correctAnswer = 0;
                int totalNumberQuestions = _dbContext.Assessment.Count(x => x.CourseId == model.CourseId && x.Status == 1);
                var questions = await _dbContext.Assessment.Where(x => x.CourseId == model.CourseId && x.Status == 1).Select(x => new VW_AssessmentQuestions
                {
                    AssessmentId = x.AssessmentId,
                    Question = x.Question,
                    Options = JsonSerializer.Deserialize<List<OptionsOfQuestions>>(x.Options, new JsonSerializerOptions()),
                }).ToListAsync();


                foreach (var item in questions)
                {
                    var answerObj = model.AssessmentAns.FirstOrDefault(x => x.AssessmentId == item.AssessmentId);
                    var answer = item.Options.FirstOrDefault(o => o.OptionNo == answerObj.SelectedOption && o.IsAnswer == true);
                    if (answer != null)
                    {
                        correctAnswer++;
                    }
                }

                //Percentage Calculation
                double percentage = (double)correctAnswer / (double)totalNumberQuestions * 100;
                string Grade = "";
                var gradeobj = await _dbContext.GradingCriteria.Where(x => x.Grade_S_R <= percentage && x.Grade_E_R >= percentage).FirstOrDefaultAsync();
                Grade = gradeobj.GradeName;
                var updateGrade = await _dbContext.UserGrade.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.CourseId == model.CourseId && x.Grade == "F");
                if (updateGrade != null)
                {
                    updateGrade.Grade = Grade;
                    updateGrade.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    _dbContext.UserGrade.Update(updateGrade);
                    var enrollemnt = await _dbContext.Enrollment.FirstOrDefaultAsync(x => x.CourseId == model.CourseId && x.UserId == model.UserId && x.EnrolledStatus != "1");
                    enrollemnt.EnrolledStatus = "1";
                    _dbContext.Enrollment.Update(enrollemnt);
                    await _dbContext.SaveChangesAsync();
                    _response.Data = updateGrade;
                    _response.Success = Constants.ResponseSuccess;
                    return _response;

                }
                else
                {

                    UserGrade usergrade = new ()
                    {
                        GradeId = Guid.NewGuid(),
                        UserId = model.UserId,
                        CourseId = model.CourseId,
                        Percentage = percentage,
                        Grade = Grade,
                        Date = DateTime.Now.ToString("dd/MM/yyyy"),
                    };
                    _dbContext.UserGrade.Add(usergrade);
                    if (Grade != "F")
                    {
                        var enrollemnt = await _dbContext.Enrollment.FirstOrDefaultAsync(x => x.CourseId == model.CourseId && x.UserId == model.UserId && x.EnrolledStatus != "1");
                        enrollemnt.EnrolledStatus = "1";
                        _dbContext.Enrollment.Update(enrollemnt);
                    }
                    await _dbContext.SaveChangesAsync();
                    _response.Data = usergrade;
                    _response.Success = Constants.ResponseSuccess;
                    return _response;
                }



            }
            _response.Message = ("You'r already Passed this Assessment.");
            _response.Success = Constants.ResponseFailure;
            return _response;
        }
        public async Task<IResponse> GetUserGrades(Guid UserId)
        {
            var userGradeList = await (from grade in _dbContext.UserGrade
                                       join course in _dbContext.Course on grade.CourseId equals course.CourseId
                                       join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                                       where (
                                           grade.UserId == UserId)
                                       select new UserGradeDetails
                                       {
                                           CourseTitle = course.Title,
                                           CategoryName = category.CategoryName,
                                           Grade = grade.Grade,
                                           Percentage = grade.Percentage,
                                           Date = grade.Date
                                       }
            ).ToListAsync();
            if (userGradeList == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Grades");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Success = Constants.ResponseSuccess;
            _response.Data = userGradeList;
            return _response;
        }
        public async Task<IResponse> GetUserGradeForCourse(Guid CourseId, Guid UserId)
        {
            var grade = await (from userGrade in _dbContext.UserGrade
                               join user in _dbContext.User on userGrade.UserId equals user.UserId
                               join course in _dbContext.Course on userGrade.CourseId equals course.CourseId
                               where (
                                  userGrade.CourseId == CourseId && userGrade.UserId == UserId)
                               select new VW_CompletedCourseGrade
                               {
                                   CourseTitle = course.Title,
                                   UserName = user.Username,
                                   Grade = userGrade.Grade,
                                   Date = userGrade.Date,
                               }).FirstOrDefaultAsync();
            if (grade == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Grades");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Success = Constants.ResponseSuccess;
            _response.Data = grade;
            return _response;
        }
    }
}