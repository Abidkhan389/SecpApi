using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Paradigm.Data.Model
{
    [Table("role")]
    public class Role
    {
        public Role()
        {

        }
        public Role(AddEditRole role, AuditTrack audit, string roleId)
        {
            this.RoleId = roleId;
            this.ParentRoleId = role.ParentRoleId;
            this.Name = role.Name;
            this.DisplayName = role.DisplayName;
            this.Enabled = true;
            this.CreatedBy = audit.UserId;
            this.CreatedOn = audit.Time;
        }
        [Key]
        public string RoleId { get; set; }
        public string ParentRoleId { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
        public int CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public int? UpdatedOn { get; set; }
        public string DisplayName { get; set; }
    }
    public class AddEditRole
    {
        public string RoleId { get; set; }
        public string ParentRoleId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
    public class VW_Role
    {
        public int TotalCount { get; set; }
        public long SerialNo { get; set; }
        [Key]
        public string RoleId { get; set; }
        public string Parent { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedOn { get; set; }
        public string CreatedDate
        {
            get
            {
                Int32 dateTime = Convert.ToInt32(CreatedOn);
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(dateTime);
                return dtDateTime.ToString("MMM dd, yyyy, hh:mm:ss tt", CultureInfo.InvariantCulture);
            }
        }
    }
}