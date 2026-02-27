using Claims.Auditing;
using Claims.Data;

public class AuditBackgroundService : BackgroundService
{
    private readonly IAuditQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuditBackgroundService(
        IAuditQueue queue,
        IServiceScopeFactory scopeFactory)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _queue.DequeueAsync(stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider
                .GetRequiredService<AuditContext>();

            if (message.ClaimId != null)
            {
                context.ClaimAudits.Add(new ClaimAudit
                {
                    ClaimId = message.ClaimId,
                    HttpRequestType = message.HttpRequestType,
                    Created = message.Created
                });
            }

            if (message.CoverId != null)
            {
                context.CoverAudits.Add(new CoverAudit
                {
                    CoverId = message.CoverId,
                    HttpRequestType = message.HttpRequestType,
                    Created = message.Created
                });
            }

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}