using DbUp;
using DbUp.Engine.Output;
using Kava.Data.DbUp;
using Kava.Models;
using Kava.Services;
using Kava.Services.Factories;
using Kava.Services.Startup;
using Kava.Utilities.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;

namespace Kava.Tests.Database;

public sealed class InsertTest : IDisposable
{
    private static readonly string TestDbPath = EnvironmentHelper.AppDirectory.JoinPath("test.db");

    private readonly SqliteConnectionFactory _connectionFactory = new($"Data Source={TestDbPath}");

    private readonly ITestOutputHelper _outputHelper;

    private readonly BoardService _boardService;

    public InsertTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _boardService = new BoardService(NullLogger<BoardService>.Instance, _connectionFactory);
    }

    [Fact]
    public async Task BoardServiceTest()
    {
        var upgrader = DeployChanges
            .To.SQLiteDatabase(_connectionFactory.ConnectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DbMigrationService).Assembly)
            .WithTransactionPerScript()
            .LogTo(new TestLogger(_outputHelper))
            .Build();

        upgrader.PerformUpgrade();

        await Task.Delay(TimeSpan.FromSeconds(5));

        using var connection = await _connectionFactory.CreateAsync();

        var board = new Board("Test Board");
        await _boardService.AddBoardAsync(board);
    }

    private class TestLogger(ITestOutputHelper outputHelper) : IUpgradeLog
    {
        public void LogTrace(string format, params object[] args)
        {
            outputHelper.WriteLine(format, args);
        }

        public void LogDebug(string format, params object[] args)
        {
            outputHelper.WriteLine(format, args);
        }

        public void LogInformation(string format, params object[] args)
        {
            outputHelper.WriteLine(format, args);
        }

        public void LogWarning(string format, params object[] args)
        {
            outputHelper.WriteLine(format, args);
        }

        public void LogError(string format, params object[] args)
        {
            outputHelper.WriteLine(format, args);
        }

        public void LogError(Exception ex, string format, params object[] args)
        {
            outputHelper.WriteLine(format, args);
        }
    }

    public void Dispose()
    {
        if (File.Exists(TestDbPath))
        {
            File.Delete(TestDbPath);
        }
    }
}
