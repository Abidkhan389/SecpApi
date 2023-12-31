using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    public class AuditService : IAuditService
    {
        private readonly DbContextBase _context;
        private Audit tempaudit;
        private ICountResponse _countResp;
        private IResponse _response;
        public AuditService(DbContextBase context, ICountResponse countResp, IResponse response)
        {
            this._context = context;
            tempaudit = new Audit();
            _countResp = countResp;
            _response = response;
        }
        public async Task<IResponse> GetAllLoginAuditsByProc(TableParamModel model, int diff)
        {
            if (String.IsNullOrEmpty(model.Sort) || String.IsNullOrEmpty(model.Order))
            {
                model.Sort = "Time";
                model.Order = "DESC";
            }
            string query = ListingQuery(model, diff);
            var data = await _context.VW_Audit.FromSqlRaw(query).ToListAsync();
            _countResp.DataList = data;
            _countResp.TotalCount = data.Count > 0 ? data.First().TotalCount : 0;
            _response.Success = Constants.ResponseSuccess;
            _response.Data = _countResp;
            return _response;
        }
        public async Task<object> AddOne(AuditTrails entity)
        {
            tempaudit.AuditId = Guid.NewGuid();
            tempaudit.Username = entity.UserName;
            tempaudit.UserId = entity.UserId;
            tempaudit.IPAddress = entity.IPAddress;
            tempaudit.DeviceType = entity.DeviceType;
            entity.Time = entity.AuditType.Equals(Constants.ActionMethods.AccountUnlocked) ? entity.Time : HelperStatic.GetCurrentTimeStamp();
            tempaudit.Time = entity.Time ?? HelperStatic.GetCurrentTimeStamp();
            tempaudit.AuditType = entity.AuditType;
            tempaudit.Location = entity.Location;
            tempaudit.City = entity.City;
            tempaudit.Country = entity.Country;
            tempaudit.OS = entity.OperatingSystem;
            if (entity.AuditType == Constants.ActionMethods.Logout)
            {
                var _lastSuccess = new Audit();
                _lastSuccess = await _context.Audit.FirstOrDefaultAsync(e => e.AuditId == entity.AuditSessionID);
                var dateOne = tempaudit.Time;
                var dateTwo = _lastSuccess != null ? _lastSuccess.Time : 0;
                System.DateTime dtDateTime1 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                System.DateTime dtDateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                TimeSpan duration = dtDateTime1.AddSeconds(Convert.ToInt32(dateOne)) - dtDateTime1.AddSeconds(Convert.ToInt32(dateTwo));
                var res = String.Format("{0}:{1}:{2}", duration.TotalHours.ToString("00"), duration.Minutes.ToString("00"), duration.Seconds.ToString("00")).ToString();
                tempaudit.SessionDuration = res;
                tempaudit.ParentAudit = _lastSuccess.AuditId;
                _context.Audit.Add(tempaudit);
                await _context.SaveChangesAsync();
                return Constants.DataSaved;
            }
            tempaudit.SessionDuration = null;
            _context.Audit.Add(tempaudit);
            await _context.SaveChangesAsync();
            return Constants.DataSaved + "*" + tempaudit.AuditId.ToString();
        }
        public string ListingQuery(TableParamModel model, int diff)
        {
            string query = "SELECT Count(audit.\"AuditId\") OVER () TotalCount, ROW_NUMBER() over(Order by (Select 1)) as SerialNo," +
            "audit.\"AuditId\"," +
            "audit.\"Username\"," +
            "audit.\"UserId\"," +
            "u1.\"DisplayName\"," +
            "audit.\"IPAddress\"," +
            "audit.\"DeviceType\"," +
            "audit.\"Time\"," +
            "audit.\"AuditType\"," +
            "audit.\"City\"" +
            "FROM \"Administration\".\"Audit\" AS audit " +
            "INNER JOIN \"Roles\".\"User\" AS u1 ON u1.\"UserId\" = audit.\"UserId\" "
            + HelperStatic.QueryFinalize(model);
            return query;
        }
    }
}