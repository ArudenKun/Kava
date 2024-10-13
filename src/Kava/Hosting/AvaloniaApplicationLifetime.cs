using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kava.Hosting;

public sealed class AvaloniaApplicationLifetime<TApplication> : IHostLifetime
    where TApplication : Application
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly TaskCompletionSource<object> _applicationExited = new();
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaApplicationLifetime{TApplication}"/> class.
    /// </summary>
    public AvaloniaApplicationLifetime(
        IHostApplicationLifetime applicationLifetime,
        IServiceProvider serviceProvider
    )
    {
        _applicationLifetime = applicationLifetime;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        var ready = new TaskCompletionSource<object>();
        using var registration = cancellationToken.Register(
            () => ready.TrySetCanceled(cancellationToken)
        );

        var application = _serviceProvider.GetRequiredService<TApplication>();

        if (application.ApplicationLifetime is IControlledApplicationLifetime desktopLifetime)
        {
            desktopLifetime.Startup += (_, _) => ready.TrySetResult(null!);
            desktopLifetime.Exit += (_, _) =>
            {
                _applicationExited.TrySetResult(null!);
                _applicationLifetime.StopApplication();
            };
        }
        else
        {
            ready.TrySetException(
                new InvalidOperationException("Generic host support classic desktop only!")
            );
        }
        await ready.Task.ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispatcher.UIThread.BeginInvokeShutdown(DispatcherPriority.Default);
        return _applicationExited.Task;
    }
}
