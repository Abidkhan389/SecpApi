namespace Paradigm.Service
{
    using System;
    using System.Text;
    using System.Linq;

    using System.Security.Claims;
    using System.Threading.Tasks;

    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;

    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using Paradigm.Contract.Interface;

    public class TokenProviderService : ITokenProviderService<Token>
    {
        private readonly TokenProviderOptions options;
        private readonly TokenProviderConfig config;

        public TokenProviderService(IOptions<TokenProviderConfig> config)
        {
            this.config = config.Value;

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.config.TokenSecurityKey));

            this.options = new TokenProviderOptions
            {
                Audience = this.config.TokenAudience,
                Issuer = this.config.TokenIssuer,
                Expiration = new TimeSpan(0, this.config.TokenExpiration, 0),
                SigningCredentials = new SigningCredentials(signingKey, this.config.TokenSecurityAlgorithm)
            };

            ThrowIfInvalidOptions(this.options);
        }

        public Task<Token> IssueToken(ClaimsIdentity identity, string subject, string sessionId, string userId)
        {
            if (identity == null)
                throw new Exception("Invalid username or password.");

            var now = DateTime.UtcNow;

            var claims = identity.Claims == null ? new List<Claim>() : identity.Claims.ToList();

            if (!string.IsNullOrWhiteSpace(subject) && !claims.Any(o => o.Type == JwtRegisteredClaimNames.Sub))
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject));

            claims.Add(new Claim("AuditSession", sessionId));
            claims.Add(new Claim("UserId", userId));

            var jwt = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(options.Expiration),
                signingCredentials: options.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var token = new Token(encodedJwt, jwt.Payload.Exp.Value);

            return Task.FromResult<Token>(token);
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }
    }
}
