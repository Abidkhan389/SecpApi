using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;
using static Paradigm.Data.Model.Lecture;

namespace Paradigm.Server.Interface
{
    public interface ILetureService
    {
        Task<IResponse> GetLecturesBycourse(Guid Id);
        Task<IResponse> GetAllByProc(List_Lectures model, int diff);
        Task<IResponse> GetLectureById(Guid Id);
        Task<IResponse> AddEditLecture(AddEditLecture model, Guid userId);
        Task<IResponse> ActiveInactive(ActiveInactive model);
        Task<IResponse> GetLectureWithContent(Guid Id, Guid UserId);
    }
}