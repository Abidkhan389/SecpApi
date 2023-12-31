namespace Paradigm.Contract.Model
{
    public interface ISignup
    {        
        string Username { get; }
        string Password { get; }
        string DisplayName { get; }
        string TimeZoneId { get; }
        string CultureName { get; }
        string[] Roles { get; }
        bool Verified { get; }
        bool Enabled { get; }
    }
}
