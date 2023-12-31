namespace Paradigm.Service.Repository
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using System.Collections.Generic;

    using Microsoft.Extensions.Options;

    using Paradigm.Data;
    using Paradigm.Data.Model;
    using Paradigm.Contract.Model;
    using Paradigm.Contract.Interface;
    using Microsoft.EntityFrameworkCore;

    public interface IUserRepository : ILocalAuthenticationService, IEntityRepository<User> { }
    public class UserRepository : EntityRepository<User>, IUserRepository
    {
        private readonly Service.Config config;
        private readonly IDeviceProfiler deviceProfiler;
        private ICryptoService crypto;
        private readonly DbContextBase db;

        public UserRepository(DbContextBase context, DbContextBase db, IOptions<Service.Config> config, ICryptoService crypto, IDeviceProfiler deviceProfiler) : base(context)
        {
            this.config = config.Value;
            this.crypto = crypto;
            this.deviceProfiler = deviceProfiler;
            this.db = db;
        }

        public async Task<ClaimsIdentity> ResolveUser(string username, string password, bool isSuperAdmin)
        {
            var login = await db.User.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

            if (login != null)
            {
                //IEnumerable<UserRole> roles = await this.FromSqlAsync<UserRole>($"SELECT ur.'RoleId', ur.'UserId' FROM 'UserRole' ur WHERE ur.'UserId' = '{login.FirstOrDefault().UserId}'");
                var roles = await db.UserRole.Where(x => x.UserId == login.UserId).ToListAsync();
                //IEnumerable<RoleSecurityClaim> claim = await this.FromSqlAsync<RoleSecurityClaim>($"SELECT DISTINCT rsc.'RoleId', rsc.'SecurityClaimId', rsc.'Value' FROM 'RoleSecurityClaim' rsc INNER JOIN 'SecurityClaim' sc ON (rsc.'SecurityClaimId' = sc.'SecurityClaimId') INNER JOIN 'UserRole' ur ON (rsc.'RoleId' = ur.'RoleId' AND ur.'UserId' = '{login.FirstOrDefault().UserId}');");
                var claims = await (
                    from roleSecurityClaim in db.RoleSecurityClaim
                    join clm in db.SecurityClaim on roleSecurityClaim.SecurityClaimId equals clm.SecurityClaimId
                    join userRole in db.UserRole on roleSecurityClaim.RoleId equals userRole.RoleId
                    where userRole.UserId == login.UserId
                    select new RoleSecurityClaim
                    {
                        RoleId = roleSecurityClaim.RoleId,
                        SecurityClaimId = roleSecurityClaim.SecurityClaimId,
                        Value = roleSecurityClaim.Value
                    }
                ).Distinct().ToListAsync();

                Model.User user = new Model.User()
                {
                    UserId = login.UserId.ToString(),
                    Username = login.Username,
                    CultureName = login.CultureName,
                    DisplayName = login.DisplayName,
                    TimeZoneId = login.TimeZoneId,
                    Enabled = login.Enabled,
                    Roles = roles,
                    Claims = claims
                };

                if (this.crypto.CheckKey(login.PasswordHash, login.PasswordSalt, password))
                {
                    string fingerprint = this.deviceProfiler.DeriveFingerprint(user);
                    ClaimsIdentity identity = user.ToClaimsIdentity(this.config.ClaimsNamespace, fingerprint);

                    return identity;
                }
            }

            return null;
        }

        public async Task<IUser> ResolveUser(string username)
        {
            var user = await this.FindByAsync($"WHERE \"Username\" = '{username}'");

            if (null != user)
            {
                return user.AsEnumerable().Select(usr => new Model.User
                {
                    UserId = usr.UserId.ToString(),
                    Username = usr.Username,
                    CultureName = usr.CultureName,
                    DisplayName = usr.DisplayName,
                    TimeZoneId = usr.TimeZoneId,
                    Enabled = usr.Enabled
                }).FirstOrDefault();
            }
            return null;
        }

        public async Task<bool> ValidateUser(string username)
        {
            var login = await this.FindByAsync($"WHERE \"Username\" = '{username}'");

            return login == null;
        }
    }
}