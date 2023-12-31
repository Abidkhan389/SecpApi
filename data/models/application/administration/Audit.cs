using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json;

namespace Paradigm.Data.Model
{
    [Table("audit")]
    public class Audit
    {
        [Key]
        public Guid AuditId { get; set; }
        public string Username { get; set; }
        public Guid? UserId { get; set; }
        public string IPAddress { get; set; }
        public string DeviceType { get; set; }
        public int Time { get; set; }
        public string AuditType { get; set; }
        public string SessionDuration { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string OS { get; set; }
        public Guid? ParentAudit { get; set; }
    }
    [Table("audithistory")]
    public class AuditHistory
    {
        public AuditHistory()
        {

        }
        public AuditHistory(Guid userId, string Username, string OldValue, string newValue, int timeStamp, Guid sessionID, string screen, string type)
        {
            this.UserId = userId;
            this.Username = Username;
            this.OldValue = OldValue;
            this.NewValue = newValue;
            this.Time = timeStamp;
            this.SessionId = sessionID;
            this.ActionScreen = screen;
            this.AuditType = type;
        }
        [Key]
        public Guid AuditHistoryId { get; set; }
        public Guid UserId { get; set; }
        #nullable enable
        public string? Username { get; set; }   
        #nullable disable
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int Time { get; set; }
        public Guid? SessionId { get; set; }
        public string ActionScreen { get; set; }
        public string AuditType { get; set; }
    }
    public class AuditTrails
    {
        [Key]
        public string Location { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string DeviceType { get; set; }
        public string IPAddress { get; set; }
        public int? Time { get; set; }
        public string OperatingSystem { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string AuditType { get; set; }
        public Guid? AuditSessionID { get; set; }
    }
    public class AuditTrack
    {
        public AuditTrack()
        {

        }
        public AuditTrack(Guid user, string username, string screen, Guid session , JsonSerializerOptions options, int time)
        {
            this.UserId = user;
            this.Username = username;
            this.ActionScreen = screen;
            this.SessionId = session;
            this.Options = options;
            this.Time = time;
        }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string ActionScreen { get; set; }
        public Guid SessionId { get; set; }
        public JsonSerializerOptions Options { get; set; }
        public int Time { get; set; }
    }
    public class VW_Audit
    {
        public int TotalCount { get; set; }
        public long SerialNo { get; set; }
        [Key]
        public Guid AuditId { get; set; }
        public string Username { get; set; }
        public Guid? UserId { get; set; }
        public string DisplayName { get; set; }
        public string IPAddress { get; set; }
        public string DeviceType { get; set; }
        public int Time { get; set; }
        public string CreatedDate
        {
            get
            {
                Int32 dateTime = Convert.ToInt32(Time);
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(dateTime);
                return dtDateTime.ToString("MMM dd, yyyy, hh:mm:ss tt", CultureInfo.InvariantCulture);
            }
        }
        public string AuditType { get; set; }
        public string City { get; set; }
    }
}