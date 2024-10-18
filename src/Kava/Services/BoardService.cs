using System.IO;
using System.Threading.Tasks;
using Dapper;
using Dapper.SimpleSqlBuilder;
using Kava.Models;
using Kava.Services.Abstractions;
using Kava.Services.Abstractions.Factories;
using Kava.Utilities.Helpers;
using Microsoft.Extensions.Logging;

namespace Kava.Services;

public class BoardService : ISingleton
{
    private static readonly string FileDbPath = EnvironmentHelper.AppDataDirectory.JoinPath(
        "kava_files.db"
    );

    private readonly ILogger<BoardService> _logger;
    private readonly IDbConnectionFactory _connectionFactory;

    public BoardService(ILogger<BoardService> logger, IDbConnectionFactory connectionFactory)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task AddBoardAsync(Board board)
    {
        var sqlBuilder = SimpleBuilder
            .CreateFluent()
            .InsertInto($"Board")
            .Columns($"Name")
            .Values($"{board.Name}");

        using var db = await _connectionFactory.CreateAsync();
        _logger.LogDebug("Inserting Board: {Sql}", sqlBuilder.Sql);
        await db.ExecuteAsync(sqlBuilder.Sql, sqlBuilder.Parameters);
    }

    public async Task<Board?> GetBoardAsync(int boardId, bool includeAllJoins = false)
    {
        using var db = await _connectionFactory.CreateAsync();

        var sqlBuilder = SimpleBuilder.CreateFluent().Select($"*").From($"Board b");

        if (includeAllJoins)
        {
            sqlBuilder
                .LeftJoin($"Category cat ON b.Id = cat.BoardId")
                .LeftJoin($"Card c ON cat.Id = c.CategoryId")
                .LeftJoin($"Attachment a ON c.Id = a.CardId");
        }

        sqlBuilder.Where($"b.Id = {boardId}");

        _logger.LogDebug("Querying Board: {Sql}", sqlBuilder.Sql);
        var result = await db.QuerySingleOrDefaultAsync<Board>(
            sqlBuilder.Sql,
            sqlBuilder.Parameters
        );

        return result;
    }

    public async Task<IEnumerable<Board>> GetBoardsAsync(bool includeCategories = false)
    {
        var sqlBuilder = SimpleBuilder.CreateFluent().Select($"*").From($"Board b");

        if (includeCategories)
        {
            sqlBuilder.InnerJoin($"Category c ON b.Id = c.BoardId");
        }

        using var db = await _connectionFactory.CreateAsync();
        _logger.LogDebug("Querying Boards: {Sql}", sqlBuilder.Sql);
        var results = await db.QueryAsync<Board>(sqlBuilder.Sql, sqlBuilder.Parameters);

        return results;
    }

    public async Task<Attachment> UploadAttachmentAsync(
        Card card,
        string attachmentName,
        byte[] attachmentBytes
    )
    {
        using var stream = new MemoryStream(attachmentBytes);
        return await UploadAttachmentAsync(card, attachmentName, stream);
    }

    public async Task<Attachment> UploadAttachmentAsync(
        Card card,
        string attachmentName,
        Stream attachmentStream
    )
    {
        var entryInfo = FileDb.Store(FileDbPath, attachmentName, attachmentStream);

        var attachment = new Attachment(
            entryInfo.FileName,
            entryInfo.FileLength,
            entryInfo.MimeType
        )
        {
            Id = new Ulid(entryInfo.ID),
            CardId = card.Id,
        };

        var sqlBuilder = SimpleBuilder
            .CreateFluent()
            .InsertInto($"Attachment")
            .Columns($"Id")
            .Columns($"CardId")
            .Columns($"Name")
            .Columns($"Size")
            .Columns($"MimeType")
            .Values($"{attachment.Id}")
            .Values($"{attachment.CardId}")
            .Values($"{attachment.Name}")
            .Values($"{attachment.Size}")
            .Values($"{attachment.MimeType}");

        using var db = await _connectionFactory.CreateAsync();
        _logger.LogDebug("Inserting Attachment: {Sql}", sqlBuilder.Sql);
        await db.ExecuteAsync(sqlBuilder.Sql, sqlBuilder.Parameters);
        return attachment;
    }

    public static ValueTask DownloadAttachmentAsync(
        Attachment attachment,
        string? destinationDirectory = null
    )
    {
        destinationDirectory ??= EnvironmentHelper.DownloadsDirectory;
        var attachmentPath = destinationDirectory.JoinPath(attachment.Name);
        FileDb.Read(FileDbPath, attachment.Id.ToGuid(), attachmentPath);
        return ValueTask.CompletedTask;
    }

    public async Task<IEnumerable<Attachment>> GetAttachmentsAsync(Card card)
    {
        var sqlBuilder = SimpleBuilder
            .CreateFluent()
            .Select($"*")
            .From($"Attachment")
            .Where($"CardId = {card.Id}");

        using var db = await _connectionFactory.CreateAsync();
        _logger.LogDebug("Querying Attachments: {Sql}", sqlBuilder.Sql);
        var results = await db.QueryAsync<Attachment>(sqlBuilder.Sql, sqlBuilder.Parameters);
        return results;
    }

    public async Task UpdateAttachmentAsync(Attachment attachment)
    {
        var sqlBuilder = SimpleBuilder
            .CreateFluent()
            .Update($"Attachment")
            .Set($"Name = {attachment.Name}")
            .Set($"Size = {attachment.Size}")
            .Set($"MimeType = {attachment.MimeType}")
            .Where($"Id = {attachment.Id}");

        using var db = await _connectionFactory.CreateAsync();
        _logger.LogDebug("Updating Attachment: {Sql}", sqlBuilder.Sql);
        await db.ExecuteAsync(sqlBuilder.Sql, sqlBuilder.Parameters);
    }

    public async Task<bool> DeleteAttachmentAsync(Attachment attachment)
    {
        var deleted = FileDb.Delete(FileDbPath, attachment.Id.ToGuid());
        FileDb.Shrink(FileDbPath);

        var sqlBuilder = SimpleBuilder
            .CreateFluent()
            .DeleteFrom($"Attachment")
            .Where($"Id = {attachment.Id}");

        using var db = await _connectionFactory.CreateAsync();
        _logger.LogDebug("Deleting Attachment: {Sql}", sqlBuilder.Sql);
        await db.ExecuteAsync(sqlBuilder.Sql, sqlBuilder.Parameters);

        return deleted;
    }
}
