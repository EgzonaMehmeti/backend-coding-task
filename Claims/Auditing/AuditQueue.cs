using System.Threading.Channels;

namespace Claims.Auditing
{
    public class AuditQueue : IAuditQueue
    {
        private readonly Channel<AuditMessage> _queue;

        public AuditQueue()
        {
            _queue = Channel.CreateUnbounded<AuditMessage>();
        }
        public void Enqueue(AuditMessage message)
        {
            if (!_queue.Writer.TryWrite(message))
            {
                throw new Exception("Unable to enqueue audit message.");
            }
        }

        public async Task<AuditMessage> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
