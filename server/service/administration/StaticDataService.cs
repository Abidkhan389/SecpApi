using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    public class StaticDataService : IStaticDataService
    {
        private DbContextBase _dbContext;
        private IResponse _response;
        private ICountResponse _countResp;
        public StaticDataService(DbContextBase dbContext, IResponse response, ICountResponse countResp)
        {
            _dbContext = dbContext;
            _response = response;
            _countResp = countResp;
        }
        public async Task<IResponse> GetAllStaticDataParentByProc(TableParamModel model, int diff)
        {
            if (String.IsNullOrEmpty(model.Sort) || String.IsNullOrEmpty(model.Order))
            {
                model.Sort = "CreatedOn";
                model.Order = "DESC";
            }
            string query = ListingQueryStaticDataParent(model, diff);
            var data = await _dbContext.VW_StaticDataParent.FromSqlRaw(query).ToListAsync();
            _countResp.DataList = data;
            _countResp.TotalCount = data.Count > 0 ? data.First().TotalCount : 0;
            _response.Success = Constants.ResponseSuccess;
            _response.Data = _countResp;
            return _response;
        }
        public async Task<IResponse> GetStaticDataParent()
        {
            var staticData = await _dbContext.StaticDataParent.Select(x => new { x.StaticDataParentId, x.StaticDataParentName }).ToListAsync();
            _response.Data = staticData;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetStaticDataByParentId(Guid parentId)
        {
            var staticData = await _dbContext.StaticData.Where(x => x.StaticDataParentId == parentId).Select(x => new { x.StaticDataId, x.ValueMember }).ToListAsync();
            _response.Data = staticData;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetStaticDataByParentName(string parentName, AuditTrack audit)
        {
            var staticDataParent = await _dbContext.StaticDataParent.FirstOrDefaultAsync(x => x.StaticDataParentName == parentName);
            if (staticDataParent == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "StaticDataParent");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            var staticData = await _dbContext.StaticData.Where(x => x.StaticDataParentId == staticDataParent.StaticDataParentId).Select(x => new { x.StaticDataId, x.ValueMember }).ToListAsync();
            _response.Data = staticData;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetMultipleStaticData(string parentName, AuditTrack audit)
        {
            List<MultipleStaticData> multipleStaticDatas = new List<MultipleStaticData>() { };
            var dataParsed = parentName.Split(',');
            var staticDataParent = await _dbContext.StaticDataParent.Where(x => dataParsed.Contains(x.StaticDataParentName)).ToListAsync();
            var parentId = staticDataParent.Select(x => x.StaticDataParentId).ToList();
            var staticData = await _dbContext.StaticData.Where(x => parentId.Contains(x.StaticDataParentId)).ToListAsync();
            foreach (var item in staticDataParent)
            {
                MultipleStaticData multipleStaticData = new MultipleStaticData();
                multipleStaticData.Parent = item.StaticDataParentName;
                multipleStaticData.Data = staticData.Where(x => x.StaticDataParentId == item.StaticDataParentId).Select(x => new IdText() { Id = x.StaticDataId, Text = x.ValueMember }).ToList();
                multipleStaticDatas.Add(multipleStaticData);
            }
            _response.Data = multipleStaticDatas;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> AddEditStaticDataParent(AddEditStaticDataParent addEdit, AuditTrack audit)
        {
            string auditOldValue = "N/A";
            string auditNewValue = "N/A";
            if (addEdit.StaticDataParentId == null)
            {
                StaticDataParent StaticData = await _dbContext.StaticDataParent.FirstOrDefaultAsync(x => x.StaticDataParentName == addEdit.StaticDataParentName);
                if (StaticData != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "StaticDataParent");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                StaticDataParent StaticDataParent = new StaticDataParent();
                StaticDataParent.StaticDataParentId = Guid.NewGuid();
                StaticDataParent.StaticDataParentName = addEdit.StaticDataParentName;
                StaticDataParent.Status = 1;
                auditNewValue = JsonSerializer.Serialize(StaticData, audit.Options);
                _dbContext.StaticDataParent.Add(StaticDataParent);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                StaticDataParent StaticData = await _dbContext.StaticDataParent.FirstOrDefaultAsync(x => x.StaticDataParentId == addEdit.StaticDataParentId);
                if (StaticData == null)
                {
                    _response.Message = Constants.NotFound.Replace("{data}", "StaticDataParent");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                auditOldValue = JsonSerializer.Serialize(StaticData, audit.Options);
                StaticDataParent Static = await _dbContext.StaticDataParent.FirstOrDefaultAsync(x => x.StaticDataParentName == addEdit.StaticDataParentName  && x.StaticDataParentId != addEdit.StaticDataParentId);
                if (Static != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "StaticDataParent");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                StaticData.StaticDataParentName = addEdit.StaticDataParentName;
                auditNewValue = JsonSerializer.Serialize(StaticData, audit.Options);
                _dbContext.StaticDataParent.Update(StaticData);
                await _dbContext.SaveChangesAsync();
            }

            var _auditLogs = new AuditHistory(audit.UserId, audit.Username, auditOldValue, auditNewValue, audit.Time, audit.SessionId, audit.ActionScreen, addEdit.StaticDataParentId == null ? Constants.ActionMethods.Add : Constants.ActionMethods.Update);
            _dbContext.AuditHistory.Add(_auditLogs);
            await _dbContext.SaveChangesAsync();

            _response.Message = Constants.DataSaved;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetSingleStaticDataParent(Guid id)
        {
            StaticDataParent StaticData = await _dbContext.StaticDataParent.FirstOrDefaultAsync(x => x.StaticDataParentId == id);
            if (StaticData == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "StaticDataParent");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            AddEditStaticDataParent addEdit = new AddEditStaticDataParent();
            addEdit.StaticDataParentId = StaticData.StaticDataParentId;
            addEdit.StaticDataParentName = StaticData.StaticDataParentName;
            _response.Data = addEdit;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> AddEdit(AddEditStaticData addEdit, AuditTrack audit)
        {
            string auditOldValue = "N/A";
            string auditNewValue = "N/A";
            if (addEdit.StaticDataId == null)
            {
                StaticData std = await _dbContext.StaticData.FirstOrDefaultAsync(x => x.ValueMember == addEdit.ValueMember && x.StaticDataParentId == addEdit.StaticDataParentId);
                if (std != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "StaticData");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                StaticData StaticData = new StaticData(addEdit);
                auditNewValue = JsonSerializer.Serialize(StaticData, audit.Options);
                _dbContext.StaticData.Add(StaticData);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                StaticData StaticData = await _dbContext.StaticData.FirstOrDefaultAsync(x => x.StaticDataId == addEdit.StaticDataId);
                if (StaticData == null)
                {
                    _response.Message = Constants.NotFound.Replace("{data}", "StaticData");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }

                StaticData std = await _dbContext.StaticData.FirstOrDefaultAsync(x => x.ValueMember == addEdit.ValueMember && x.StaticDataParentId == addEdit.StaticDataParentId && x.StaticDataId != addEdit.StaticDataId);
                if (std != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "StaticData");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }

                auditOldValue = JsonSerializer.Serialize(StaticData, audit.Options);
                StaticData.StaticDataParentId = addEdit.StaticDataParentId;
                StaticData.ValueMember = addEdit.ValueMember;
                _dbContext.StaticData.Update(StaticData);
                await _dbContext.SaveChangesAsync();
                auditNewValue = JsonSerializer.Serialize(StaticData, audit.Options);
            }

            var _auditLogs = new AuditHistory(audit.UserId, audit.Username, auditOldValue, auditNewValue, audit.Time, audit.SessionId, audit.ActionScreen, addEdit.StaticDataId == null ? Constants.ActionMethods.Add : Constants.ActionMethods.Update);
            _dbContext.AuditHistory.Add(_auditLogs);
            await _dbContext.SaveChangesAsync();

            _response.Message = Constants.DataSaved;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetSingle(Guid id)
        {
            StaticData StaticData = await _dbContext.StaticData.FirstOrDefaultAsync(x => x.StaticDataId == id);
            if (StaticData == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "StaticData");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            AddEditStaticData addEdit = new AddEditStaticData();
            addEdit.StaticDataId = StaticData.StaticDataId;
            addEdit.StaticDataParentId = StaticData.StaticDataParentId;
            addEdit.ValueMember = StaticData.ValueMember;
            _response.Data = addEdit;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public string ListingQueryStaticDataParent(TableParamModel model, int diff)
        {
            string query = "SELECT Count(parnt.\"StaticDataParentId\") OVER () TotalCount, ROW_NUMBER() over(Order by (Select 1)) as SerialNo," +
            "parnt.\"StaticDataParentId\"," +
            "parnt.\"StaticDataParentName\"," +
            "parnt.\"Status\" " +
            "FROM \"Administration\".\"StaticDataParent\" AS parnt " 
            + HelperStatic.QueryFinalize(model);
            return query;
        }
    }
}