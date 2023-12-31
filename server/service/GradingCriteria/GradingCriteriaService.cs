
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
    public class GradingCriteriaService : IGradingCriteria
    {
        private DbContextBase _dbContext;
        private IResponse _response;
        private readonly ICountResponse _countResp;

        public GradingCriteriaService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            _dbContext = dbContext;
            _response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> AddEditGradingCriteria(VW_AddEditGradingCriteria model, Guid userId)
        {
            if (model.GradingId == null)
            {
                // Check if the grade name already exists
                var gradingCriteria = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.GradeName == model.GradeName);
                if (gradingCriteria != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "GradeName");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                // Check if Grade_S_R is lesser than Grade_E_R or both are same
                if (model.Grade_S_R >= model.Grade_E_R || model.Grade_S_R == model.Grade_E_R)
                {
                    _response.Message = "Invalid Input";
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                // Check if there exists any grading criteria with the same Grade_S_R and Grade_E_R for a different GradeName
                var grade = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.Grade_S_R == model.Grade_S_R && x.Grade_E_R == model.Grade_E_R && x.GradeName != model.GradeName);
                if (grade != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "Grade_S_R and Grade_E_R for a different GradeName");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                // Check if there exists any grading criteria with the same Grade_S_R or Grade_E_R for a different GradeName
                var gradeCheck = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.Grade_S_R == model.Grade_S_R || x.Grade_E_R == model.Grade_E_R && x.GradeName != model.GradeName);
                if (gradeCheck != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "Grade_S_R or Grade_E_R for a different GradeName");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }


                var gradingCriteriaObj = new GradingCriteria(model, userId, HelperStatic.GetCurrentTimeStamp());

                await _dbContext.GradingCriteria.AddAsync(gradingCriteriaObj);
                await _dbContext.SaveChangesAsync();
                _response.Data = gradingCriteriaObj;
                _response.Message = Constants.DataSaved;
                _response.Success = Constants.ResponseSuccess;
                return _response;
            }

            var addEditGradingCriteria = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.GradingId == model.GradingId);
            if (addEditGradingCriteria == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "GradingCriteria");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            // Check if Grade_S_R is lesser than Grade_E_R or both are same
            if (model.Grade_S_R >= model.Grade_E_R || model.Grade_S_R == model.Grade_E_R)
            {
                _response.Message = "Invalid Input";
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            // Check if the grade name already exists and is not the same as the current one
            var gradingCriteriaWithSameName = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.GradeName == model.GradeName && x.GradeName != addEditGradingCriteria.GradeName);
            if (gradingCriteriaWithSameName != null)
            {
                _response.Message = Constants.Exists.Replace("{data}", "GradeName");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }

            // Check if there exists any grading criteria with the same Grade_S_R and Grade_E_R for a different GradeName
            var gradingCriteriaWithSameRange = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.Grade_S_R == model.Grade_S_R && x.Grade_E_R == model.Grade_E_R && x.GradeName != addEditGradingCriteria.GradeName);
            if (gradingCriteriaWithSameRange != null)
            {
                _response.Message = Constants.Exists.Replace("{data}", "Grade_S_R and Grade_E_R for a different GradeName");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            // Check if there exists any grading criteria with the same Grade_S_R or Grade_E_R for a different GradeName
            var gradeChecks = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.Grade_S_R == model.Grade_S_R || x.Grade_E_R == model.Grade_E_R && x.GradeName != model.GradeName);
            if (gradeChecks != null)
            {
                _response.Message = Constants.Exists.Replace("{data}", "Grade_S_R or Grade_E_R for a different GradeName");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }

            addEditGradingCriteria.GradeName = model.GradeName;
            addEditGradingCriteria.Grade_S_R = model.Grade_S_R;
            addEditGradingCriteria.Grade_E_R = model.Grade_E_R;
            addEditGradingCriteria.UpdatedOn = HelperStatic.GetCurrentTimeStamp();
            addEditGradingCriteria.UpdatedBy = userId;

            _dbContext.GradingCriteria.Update(addEditGradingCriteria);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            _response.Message = Constants.DataUpdate;
            return _response;
        }


        public async Task<IResponse> GetAllByProc(List_GradingCriteria model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "GradeName" : model.Sort;
            //LINQ
            var data = (from GradingCriteria in _dbContext.GradingCriteria
                        join createdBy in _dbContext.User on GradingCriteria.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on GradingCriteria.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        where (
                         (EF.Functions.ILike(GradingCriteria.GradeName, $"%{model.GradeName}%") || String.IsNullOrEmpty(model.GradeName))

                         )
                        select new VW_GradingCriteria
                        {
                            GradingId = GradingCriteria.GradingId,
                            GradeName = GradingCriteria.GradeName,
                            Grade_E_R = GradingCriteria.Grade_E_R,
                            Grade_S_R = GradingCriteria.Grade_S_R
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

        public async Task<IResponse> GetGradingCriteriaById(Guid id)
        {
            var gradingCriteria = await _dbContext.GradingCriteria.FirstOrDefaultAsync(x => x.GradingId == id);
            if (gradingCriteria == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "GradingCriteria");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = gradingCriteria;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
    }
}