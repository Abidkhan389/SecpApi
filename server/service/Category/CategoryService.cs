
using System.Threading.Tasks;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Paradigm.Server.Application
{
    public class CategoryService : ICategoryService
    {
        private DbContextBase _dbContext;
        private IResponse _response;
        private readonly ICountResponse _countResp;

        public CategoryService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            _dbContext = dbContext;
            _response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetAllCategories()
        {

            var category = await _dbContext.Category.Where(x => x.Status == 1)
           .Select(x => new Count_Category
           {
               CategoryName = x.CategoryName,
               CategoryId = x.CategoryId,
               Count = _dbContext.Course.Where(y => y.CategoryId == x.CategoryId && y.Status == 1).Count(),
           }).ToListAsync();
            _response.Data = category;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetAllByProc(List_Categories model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "CategoryName" : model.Sort;
            //LINQ
            var data = (from Category in _dbContext.Category
                        join createdBy in _dbContext.User on Category.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on Category.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        where (
                         (EF.Functions.ILike(Category.CategoryName, $"%{model.CategoryName}%") || String.IsNullOrEmpty(model.CategoryName))
                         && (Category.Status == model.Status || model.Status == null))
                        select new VW_Category
                        {
                            CategoryId = Category.CategoryId,
                            CategoryName = Category.CategoryName,
                            Status = Category.Status,
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

        public async Task<IResponse> GetCategoryById(Guid id)
        {
            var category = await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryId == id);
            _response.Data = category;
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }
        public async Task<IResponse> AddEditCategory(AddEditCategory model)
        {
            if (model.CategoryId == null)
            {
                var category = await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryName.ToLower() == model.CategoryName.ToLower());
                if (category != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "CategoryName");
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }
                var categoryObj = new Category(model);

                await _dbContext.Category.AddAsync(categoryObj);
                await _dbContext.SaveChangesAsync();
                _response.Data = categoryObj;
                _response.Message = Constants.DataSaved;
                _response.Success = Constants.ResponseSuccess;
                return _response;
            }
            var AddEditCategory = await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryId == model.CategoryId);
            if (AddEditCategory == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Category");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            AddEditCategory.CategoryName = model.CategoryName;
            if (AddEditCategory.CategoryName.ToLower() == model.CategoryName.ToLower())
            {
                _response.Message = Constants.Exists.Replace("{data}", "Category");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            AddEditCategory.UpdatedOn = model.UpdatedOn;
            AddEditCategory.UpdatedBy = model.UpdatedBy;

            _dbContext.Category.Update(AddEditCategory);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            _response.Message = Constants.DataUpdate;
            return _response;
        }

        public async Task<IResponse> ActiveInactive(ActiveInactive model)
        {
            Category category = await _dbContext.Category.FirstOrDefaultAsync(x => x.CategoryId == model.Id);
            if (category == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Category");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            category.Status = model.Status;
            _dbContext.Category.Update(category);
            var course = await _dbContext.Course.Where(x => x.CategoryId == model.Id).ToListAsync();
            foreach (var item in course)
            {
                item.Status = model.Status;
                var lecture = await _dbContext.Lecture.Where(x => x.CourseId == item.CourseId).ToListAsync();
                foreach (var items in lecture)
                {
                    items.Status = model.Status;
                }
                _dbContext.Lecture.UpdateRange(lecture);
                var courseContent = await _dbContext.CourseContent.Where(x => x.CourseId == item.CourseId).ToListAsync();
                foreach (var content in courseContent)
                {
                    content.Status = model.Status;
                }
                _dbContext.CourseContent.UpdateRange(courseContent);
                var quiz = await _dbContext.Quiz.Where(x => x.CourseId == item.CourseId).ToListAsync();
                foreach (var question in quiz)
                {
                    question.Status = model.Status;
                }
                _dbContext.Quiz.UpdateRange(quiz);
            }
            _dbContext.Course.UpdateRange(course);


            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetEnrolledCategories(Guid UserId)
        {
            var categoryList = await (from category in _dbContext.Category
                                      join course in _dbContext.Course on category.CategoryId equals course.CategoryId
                                      join enrollment in _dbContext.Enrollment on course.CourseId equals enrollment.CourseId
                                      where (
                                        enrollment.UserId == UserId && enrollment.EnrolledStatus == "0")
                                      select new VW_Category
                                      {
                                          CategoryId = category.CategoryId,
                                          CategoryName = category.CategoryName,
                                      }).Distinct().ToListAsync();
            if (categoryList == null || !categoryList.Any())
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Categories");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Success = Constants.ResponseSuccess;
            _response.Data = categoryList;
            return _response;

        }

    }
}
