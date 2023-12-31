namespace Paradigm.Contract.Model
{
    public interface IAuditEvent
    {
        int AuditEventTypeId { get; }
        string Message { get; }
    }
}