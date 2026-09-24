using MediaHub.Application.Services;

namespace MediaHub.Api.BackgroundServices;

public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundJobQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(
        IBackgroundJobQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<QueuedHostedService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _queue.DequeueAsync(stoppingToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                await workItem(scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar job em background.");
            }
        }
    }
}