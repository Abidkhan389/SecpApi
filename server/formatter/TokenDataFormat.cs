namespace Paradigm.Server.Formatter
{
    using System;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;

    using Microsoft.IdentityModel.Tokens;
    using Microsoft.AspNetCore.Authentication;
    
    public class TokenDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string algorithm;
        private readonly string scheme;
        private readonly TokenValidationParameters validationParameters;

        public TokenDataFormat(string algorithm, string scheme, TokenValidationParameters validationParameters)
        {
            this.algorithm = algorithm;
            this.scheme = scheme;
            this.validationParameters = validationParameters;
        }

        public AuthenticationTicket Unprotect(string protectedText)
            => Unprotect(protectedText, null);

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken validToken = null;

            try
            {
                principal = handler.ValidateToken(protectedText, this.validationParameters, out validToken);

                var validJwt = validToken as JwtSecurityToken;

                if (validJwt == null)
                    throw new ArgumentException("Invalid JWT");

                if (!validJwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal))
                    throw new ArgumentException($"Algorithm must be '{algorithm}'");
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }

            return new AuthenticationTicket(principal, new AuthenticationProperties(), this.scheme);
        }

        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }
    }
}
