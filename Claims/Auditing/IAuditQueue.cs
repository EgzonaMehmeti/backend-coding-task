namespace Claims.Auditing
{
    public interface IAuditQueue
    {
        void Enqueue(AuditMessage message);
        Task<AuditMessage> DequeueAsync(CancellationToken cancellationToken);
    }
}
