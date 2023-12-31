namespace Paradigm.Contract.Model
{
    public interface ISignupServiceOptions
    {
        string CultureName { get; }
        string Username { get; }
        bool Enabled { get; }
        string DisplayName { get; }
        string Password { get; }
        string TimeZoneId { get; }
        bool Verified { get; }
        string[] Roles { get; }
    }
}
