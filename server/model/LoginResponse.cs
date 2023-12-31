using System;
namespace Paradigm.Server.Model
{

    public class LogInResponse
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
    }
}