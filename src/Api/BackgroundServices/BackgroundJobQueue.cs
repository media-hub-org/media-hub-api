using System.Threading.Channels;
using MediaHub.Application.Services;

namespace MediaHub.Api.BackgroundServices;

public class BackgroundJobQueue : IBackgroundJobQueue
{
    private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue =
        Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

    public void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        _queue.Writer.TryWrite(workItem);
    }

    public async Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}