namespace Paradigm.Data
{
    using System;
    using System.Linq;
    
    using Paradigm.Contract;
    using Paradigm.Data.Model;
    using Paradigm.Contract.Constant;
    using Paradigm.Contract.Interface;

    public static partial class Extensions
    {
        private const string AdminEmail = "admin@paradigm.org";

        public static void EnsureSeedData(this DbContextBase db, ICryptoService crypto)
        {
            EnsureLocalProvider(db);
            EnsureExternalProviders(db);
            EnsureAdmin(db, crypto);
        }

        private static Provider EnsureLocalProvider(DbContextBase db)
        {
            Provider provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Local);

            if (provider == null)
            {
                provider = new Provider()
                {
                    ProviderId = ProviderTypes.Local,
                    Name = "Site",
                    Description = "Authenticate with a username/password provider by this site",
                    Enabled = true
                };

                db.Provider.Add(provider);
                db.SaveChanges();
            }

            return provider;
        }

        private static Provider EnsureExternalProviders(DbContextBase db)
        {
            Provider provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Google);

            if (provider == null)
            {
                provider = new Provider()
                {
                    ProviderId = ProviderTypes.Google,
                    Name = "Google",
                    Description = "Logon using your google account",
                    Enabled = true
                };

                db.Provider.Add(provider);
                db.SaveChanges();
            }

            provider = db.Provider.FirstOrDefault(o => o.ProviderId == ProviderTypes.Microsoft);

            if (provider == null)
            {
                provider = new Provider()
                {
                    ProviderId = ProviderTypes.Microsoft,
                    Name = "Microsoft",
                    Description = "Logon using your microsoft account",
                    Enabled = true
                };

                db.Provider.Add(provider);
                db.SaveChanges();
            }

            return provider;
        }

        private static User EnsureAdmin(DbContextBase db, ICryptoService crypto)
        {
            User admin = db.User.SingleOrDefault(o => o.Username == AdminEmail);
            Guid userId = new Guid();

            string[] claims = new string[]
            {
                SecurityClaimTypes.Example
            };

            if (admin == null)
            {                
                string salt = crypto.CreateSalt();
                string hash = crypto.CreateKey(salt, "P@ssw0rd");

                admin = new User()
                {
                    UserId = userId,
                    CultureName = "en",
                    DisplayName = "Administrator",
                    Enabled = true,
                    TimeZoneId = Globalization.DefaultTimeZoneId,
                    Username = AdminEmail,
                    ProviderId = ProviderTypes.Local,
                    PasswordSalt = salt,
                    PasswordHash = hash,
                    IsSuperAdmin = false
                };

                db.User.Add(admin);
                db.SaveChanges();
            }

            Role role = db.Role.FirstOrDefault(o => o.RoleId == RoleTypes.Admin);
            User user = db.User.FirstOrDefault(o => o.Username == AdminEmail);

            if (role == null)
            {
                string name = RoleTypes.System.FirstOrDefault(o => o.Key == RoleTypes.Admin).Value;

                role = new Role()
                {
                    Enabled = true,
                    Name = name,
                    RoleId = RoleTypes.Admin
                };

                db.Role.Add(role);
                db.SaveChanges();
            }

            if (!db.UserRole.Any())
            {
                var userRole = new UserRole()
                {
                    RoleId = role.RoleId,
                    UserId = user.UserId
                };

                db.UserRole.Add(userRole);
                db.SaveChanges();
            }

            foreach (string claim in claims)
            {
                var securityClaim = db.SecurityClaim.FirstOrDefault(o => o.SecurityClaimId == claim);

                if (securityClaim == null)
                {
                    securityClaim = new SecurityClaim()
                    {
                        SecurityClaimId = claim,
                        Description = claim,
                        Enabled = true,
                        Origin = "System",
                        ValidationPattern = SecurityClaimTypes.AllowedValuesPattern,                        
                    };

                    db.SecurityClaim.Add(securityClaim);
                    db.SaveChanges();

                    var roleSecurityClaim = new RoleSecurityClaim()
                    {
                        RoleId = role.RoleId,
                        SecurityClaimId = claim,
                        Value = SecurityClaimTypes.AllowedValuesPattern
                    };
                    
                    db.RoleSecurityClaim.Add(roleSecurityClaim);

                    db.SaveChanges();
                }
            }

            return admin;
        }     
    }
}