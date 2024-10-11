using Kava.Core.Helpers;
using Kava.Core.Models.Entities;
using Kava.Services;
using Xunit.Abstractions;

namespace Kava.Tests.Database;

public class InsertTest
{
    private readonly ITestOutputHelper _outputHelper;

    public InsertTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task InsertFullHierarchy()
    {
        var mocker = new AutoMocker();
        using var factory = mocker.WithDbScope();
        await using var dbContext = await factory.CreateDbContextAsync();
        var salt = Random.Shared.Next().GetMD5Hash();
        var board = new Board { Name = $"Test Board-{salt}" };

        dbContext.Boards.Add(board);
        await dbContext.SaveChangesAsync();

        var board2 = await dbContext.Boards.FindAsync(board.Id);

        Assert.Equivalent(board, board2);
    }
}
