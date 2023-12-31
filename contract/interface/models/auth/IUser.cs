namespace Paradigm.Contract.Model
{
    public interface IUser
    {
        string UserId { get; }
        string Username { get; }
        string CultureName { get; }
        string DisplayName { get; }
        string Email { get; }
        string TimeZoneId { get; }
        bool Enabled { get; set; }        
    }
}