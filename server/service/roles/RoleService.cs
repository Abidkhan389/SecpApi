using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    public class RoleService : IRoleService
    {
        private DbContextBase _dbContext;
        private IResponse _response;
        private ICountResponse _countResp;
        public RoleService(DbContextBase dbContext, IResponse response, ICountResponse countResp)
        {
            _dbContext = dbContext;
            _response = response;
            _countResp = countResp;
        }
        public async Task<IResponse> GetAll()
        {
            var data = await _dbContext.Role.Select(x => new { Id = x.RoleId, Text = x.Name }).ToListAsync();
            _response.Data = data;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetAllByProc(TableParamModel model, int diff)
        {
            if (String.IsNullOrEmpty(model.Sort) || String.IsNullOrEmpty(model.Order))
            {
                model.Sort = "CreatedOn";
                model.Order = "DESC";
            }
            string query = ListingQuery(model, diff);
            var data = await _dbContext.VW_Role.FromSqlRaw(query).ToListAsync();
            _countResp.DataList = data;
            _countResp.TotalCount = data.Count > 0 ? data.First().TotalCount : 0;
            _response.Success = Constants.ResponseSuccess;
            _response.Data = _countResp;
            return _response;
        }
        public async Task<IResponse> AddEdit(AddEditRole addEdit, AuditTrack audit)
        {
            if (String.IsNullOrEmpty(addEdit.RoleId))
            {
                string roleId = addEdit.Name.First().ToString().ToLower() + addEdit.Name.Substring(1);
                roleId = Regex.Replace(roleId, @"\s", "") + HelperStatic.RandomString(5);
                Role Role = new Role(addEdit, audit, roleId);
                _dbContext.Role.Add(Role);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Role Role = await _dbContext.Role.FirstOrDefaultAsync(x => x.RoleId == addEdit.RoleId);
                if (Role == null)
                {
                    _response.Message = Constants.NotFound.Replace("{data}", "Role");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                Role.Name = addEdit.Name;
                Role.ParentRoleId = addEdit.ParentRoleId;
                Role.DisplayName = addEdit.DisplayName;
                Role.UpdatedBy = audit.UserId;
                Role.UpdatedOn = audit.Time;
                _dbContext.Role.Update(Role);
                await _dbContext.SaveChangesAsync();
            }
            _response.Message = Constants.DataSaved;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetSingle(string id)
        {
            Role Role = await _dbContext.Role.FirstOrDefaultAsync(x => x.RoleId == id);
            if (Role == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Role");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            AddEditRole addEdit = new AddEditRole();
            addEdit.RoleId = Role.RoleId;
            addEdit.Name = Role.Name;
            addEdit.ParentRoleId = Role.ParentRoleId;
            addEdit.DisplayName = Role.DisplayName;
            _response.Data = addEdit;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> ActiveInactive(ActiveInactiveRole model, AuditTrack audit)
        {
            string auditOldValue = "N/A";
            string auditNewValue = "N/A";
            Role role = await _dbContext.Role.FirstOrDefaultAsync(x => x.RoleId == model.Id);
            if (role == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Role");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            auditOldValue = JsonSerializer.Serialize(role, audit.Options);
            role.Enabled = model.Status;
            _dbContext.Role.Update(role);
            await _dbContext.SaveChangesAsync();
            auditNewValue = JsonSerializer.Serialize(role, audit.Options);

            var _auditLogs = new AuditHistory(audit.UserId, audit.Username, auditOldValue, auditNewValue, audit.Time, audit.SessionId, audit.ActionScreen, model.Status == true ? Constants.ActionMethods.Active : Constants.ActionMethods.Deactive);
            _dbContext.AuditHistory.Add(_auditLogs);
            await _dbContext.SaveChangesAsync();

            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public string ListingQuery(TableParamModel model, int diff)
        {
            string query = "SELECT Count(Role.\"RoleId\") OVER () TotalCount, ROW_NUMBER() over(Order by (Select 1)) as SerialNo," +
            "Role.\"RoleId\"," +
            "r1.\"Name\" AS \"Parent\"," +
            "Role.\"Name\"," +
            "Role.\"DisplayName\"," +
            "Role.\"Enabled\"," +
            "COALESCE(u1.\"DisplayName\", u.\"DisplayName\") AS \"CreatedBy\"," +
            "(COALESCE(Role.\"UpdatedOn\", Role.\"CreatedOn\") +" + diff + ") AS \"CreatedOn\" " +
            "FROM \"Roles\".\"Role\" AS Role " +
            "INNER JOIN \"Roles\".\"User\" AS u ON u.\"UserId\" = Role.\"CreatedBy\" " +
            "LEFT OUTER JOIN \"Roles\".\"User\" AS u1 ON u1.\"UserId\" = Role.\"UpdatedBy\" " +
            "LEFT OUTER JOIN \"Roles\".\"Role\" AS r1 ON r1.\"RoleId\" = Role.\"ParentRoleId\" "
            + HelperStatic.QueryFinalize(model);
            return query;
        }
    }
}