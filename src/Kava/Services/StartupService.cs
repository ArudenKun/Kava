using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kava.Services;

public sealed class StartupService : IHostedService
{
    private readonly ILogger<StartupService> _logger;
    private readonly IFreeSql _freeSql;
    private readonly SettingsService _settingsService;

    public StartupService(
        ILogger<StartupService> logger,
        IFreeSql freeSql,
        SettingsService settingsService
    )
    {
        _logger = logger;
        _freeSql = freeSql;
        _settingsService = settingsService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _settingsService.Load();
        var currentMode = (string)
            await _freeSql.Ado.ExecuteScalarAsync(
                "PRAGMA journal_mode;",
                cancellationToken: cancellationToken
            );

        if (!string.Equals(currentMode, "WAL", StringComparison.OrdinalIgnoreCase))
        {
            await _freeSql.Ado.ExecuteNonQueryAsync(
                "PRAGMA journal_mode = WAL;",
                cancellationToken: cancellationToken
            );
            _logger.LogInformation("WAL mode enabled.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _settingsService.Save();
        return Task.CompletedTask;
    }
}
