using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Kava.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kava.Services.Hosting;

public class StartupHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public StartupHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    [RequiresDynamicCode("Calls DbContext.Database.MigrateAsync")]
#pragma warning disable IL3051
    public async Task StartAsync(CancellationToken cancellationToken)
#pragma warning restore IL3051
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync(cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
