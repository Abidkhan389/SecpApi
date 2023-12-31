namespace Paradigm.Service
{
    using System;    
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Collections.Generic;

    using Paradigm.Service.Model;
    using Paradigm.Service.Security;
    using Paradigm.Contract.Constant;

    public static partial class Extensions
    {
        public static ClaimsIdentity ToClaimsIdentity(this User user, string ns, string fingerPrint)
        {
            List<Claim> claims = new List<Claim>();

            // standard oid claims
            claims.AddRange(MapOpenIdClaims(user, claims));

            // vendor oid claims - used by microsoft
            claims.AddRange(MapRoleClaims(user));

            // custom authorization claims
            claims.AddRange(MapAuthorizationClaims(user, ns));

            // custom user profile data
            claims.AddRange(MapProfileClaims(user, ns, fingerPrint));

            var identity = new ClaimsIdentity(new GenericIdentity(user.Username, "Token"), claims.ToArray());

            return identity;
        }

        private static IEnumerable<Claim> MapProfileClaims(User user, string ns, string fingerPrint)
        {
            bool isVerified = user.Roles.Any(o => o.RoleId == RoleTypes.Admin);  // admin users never have to go through account verification process

            if (!isVerified)
                isVerified = !string.IsNullOrWhiteSpace(fingerPrint) && user.Verifications != null;

            return new Claim[]
            {
                new Claim(ns + ProfileClaimTypes.CultureName, user.CultureName),
                new Claim(ns + ProfileClaimTypes.TimeZoneId, user.TimeZoneId),
                new Claim(ns + ProfileClaimTypes.Fingerprint, fingerPrint ?? "None"),
                new Claim(ns + ProfileClaimTypes.Verified, isVerified ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())
            };
        }

        private static IEnumerable<Claim> MapOpenIdClaims(User user, List<Claim> claims)
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Username),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Sid, user.UserId.ToString())
            };
        }

        private static IEnumerable<Claim> MapRoleClaims(User user)
        {
            return user.Roles.Select(o => new Claim(ClaimTypes.Role, o.RoleId));
        }

        private static IEnumerable<Claim> MapAuthorizationClaims(User user, string ns)
        {
            return user.Claims.Select(o => new Claim(ns + o.SecurityClaimId, o.Value));
        }

        public static User FromClaimsPrincipal(this ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                string userId = principal.TryGetClaimValue<string>(ClaimTypes.Sid);

                if (userId.Length > 10)
                {
                    return new User()
                    {
                        CultureName = principal.TryGetClaimValue<string>(ProfileClaimTypes.CultureName),
                        DisplayName = principal.Identity.Name,
                        Enabled = true,
                        Username = principal.TryGetClaimValue<string>(ClaimTypes.Email),
                        UserId = userId,
                        TimeZoneId = principal.TryGetClaimValue<string>(ProfileClaimTypes.TimeZoneId)
                    };
                }
            }

            return null;
        }

        public static long UserId(this ClaimsPrincipal principal)
        {
            long userId = 0;

            if (principal.Identity.IsAuthenticated)
                userId = principal.TryGetClaimValue<long>(ClaimTypes.Sid);

            return userId;
        }

        public static T TryGetClaimValue<T>(this ClaimsPrincipal principal, string type)
        {
            if (!principal.HasClaim(o => o.Type == type))
                return default(T);

            string value = principal.Claims.FirstOrDefault(o => o.Type == type).Value;

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
