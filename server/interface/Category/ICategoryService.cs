using System;
using System.Threading.Tasks;
using Paradigm.Server.Application;
using Paradigm.Data.Model;

namespace Paradigm.Server.Interface
{
    public interface ICategoryService
    {
        Task<IResponse> GetAllCategories();
        Task<IResponse> GetAllByProc(List_Categories model, int diff);
        Task<IResponse> ActiveInactive(ActiveInactive model);
        Task<IResponse> GetCategoryById(Guid id);
        Task<IResponse> AddEditCategory(AddEditCategory model);
        Task<IResponse> GetEnrolledCategories(Guid UserId);

    }
}