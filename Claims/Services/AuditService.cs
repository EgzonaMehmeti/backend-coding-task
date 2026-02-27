using Claims.Auditing;
using Claims.Services.Interfaces;

namespace Claims.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditQueue _queue;

        public AuditService(IAuditQueue queue)
        {
            _queue = queue;
        }

        public Task AuditClaimAsync(string id, string requestType)
        {
            _queue.Enqueue(new AuditMessage
            {
                ClaimId = id,
                Created = DateTime.UtcNow,
                HttpRequestType = requestType
            });

            return Task.CompletedTask;
        }

        public Task AuditCoverAsync(string id, string requestType)
        {
            _queue.Enqueue(new AuditMessage
            {
                CoverId = id,
                Created = DateTime.UtcNow,
                HttpRequestType = requestType
            });

            return Task.CompletedTask;
        }
    }
}
