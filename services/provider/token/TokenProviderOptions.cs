﻿namespace Paradigm.Service
{    
    using System;
    using System.Threading.Tasks;

    using Microsoft.IdentityModel.Tokens;

    public class TokenProviderOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);

        public SigningCredentials SigningCredentials { get; set; }

        public Func<Task<string>> NonceGenerator { get; set; } = new Func<Task<string>>(() => Task.FromResult(Guid.NewGuid().ToString()));
    }
}
