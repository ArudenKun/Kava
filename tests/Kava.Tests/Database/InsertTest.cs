using System.Globalization;
using Kava.Core.Models;
using Kava.Helpers;
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
        await Task.Delay(TimeSpan.FromSeconds(2));
        board.Name = $"Test Board2-{salt}";
        dbContext.Update(board);
        await dbContext.SaveChangesAsync();
        _outputHelper.WriteLine(board.CreatedAt.ToString(CultureInfo.InvariantCulture));
        _outputHelper.WriteLine(board.UpdatedAt.ToString(CultureInfo.InvariantCulture));
    }

    // [Fact]
    // public async Task FileDbInsertionTest()
    // {
    //     const string fileDbPath = @"C:\Users\alden\AppData\Roaming\Kava\file_store.db";
    //     var fileName = $"test-{DateTime.Now:yyyyMMddHHmmssfff}";
    //     var fileInfo = await Task.Run(async () =>
    //     {
    //         await using var fileStream = await FileSystemHelper.OpenReadAsync(
    //             @"C:\Users\alden\Downloads\test.txt"
    //         );
    //         return DB.Store(fileDbPath, fileName, fileStream);
    //     });
    //
    //     _outputHelper.WriteLine($"{fileInfo.FileLength.Bytes()}");
    //     Assert.Equal(fileInfo.FileName, fileName);
    // }
}
