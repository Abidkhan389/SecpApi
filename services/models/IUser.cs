namespace Paradigm.Service.Model
{
    using System.Collections.Generic;

    using Paradigm.Data.Model;
    using Paradigm.Contract.Model;    

    public partial class User : IUser
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string CultureName { get; set; }
        public string DisplayName { get; set; }
        public string Email
        {
            get
            {
                return this.Username;
            }
        }

        public string TimeZoneId { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<UserRole> Roles { get; set; }
        public IEnumerable<RoleSecurityClaim> Claims { get; set; }
        public string Verifications { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}