using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Paradigm.Data.Model;
using Paradigm.Server.Application;
using System.Collections.Generic;

namespace Paradigm.Server.Interface
{
    public interface IQuizService
    {
       
        Task<IResponse> GetAllCourses();
        Task<IResponse> GetAllLectures(Guid Id);
        Task<IResponse> GetQuestionById(Guid Id);
        Task<IResponse> ActiveInactive(ActiveInactive model);
        Task<IResponse> GetAllByProc(List_Quiz model, int diff);
        Task<IResponse> AddEditQuestion(VW_Quizaddedit model, Guid Id);
        Task<IResponse> GetQuestionsByLecture(Guid courseId, Guid lectureId);
    }
}