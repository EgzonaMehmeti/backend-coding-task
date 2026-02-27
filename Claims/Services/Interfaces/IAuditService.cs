namespace Claims.Services.Interfaces
{
    public interface IAuditService
    {
        Task AuditClaimAsync(string id, string requestType);
        Task AuditCoverAsync(string id, string requestType);
    }
}
