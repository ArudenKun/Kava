using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DbUp;
using Kava.Data.Converters;
using Kava.Data.DbUp;
using Kava.Services.Abstractions.Factories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kava.Services.Startup;

public class DbMigrationService : IHostedLifecycleService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DbMigrationService> _logger;

    public DbMigrationService(
        IDbConnectionFactory connectionFactory,
        ILogger<DbMigrationService> logger
    )
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        SqlMapper.AddTypeHandler(new StringUlidHandler());

        var upgrader = DeployChanges
            .To.SQLiteDatabase(_connectionFactory.ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransactionPerScript()
            .LogTo(_logger)
            .Build();

        if (!upgrader.IsUpgradeRequired())
        {
            _logger.LogInformation("No database changes. Skipping database migration.");
            return Task.CompletedTask;
        }

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Database migration failed.");
            _logger.LogError("Script Failed to execute: {ScriptName}", result.ErrorScript.Name);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Database migration successful.");
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
