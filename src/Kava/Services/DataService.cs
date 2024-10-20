using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kava.Data.Abstractions;
using Kava.Models;
using Kava.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kava.Services;

public class DataService : ISingleton
{
    private readonly ILogger<DataService> _logger;
    private readonly IRepository _repository;

    public DataService(ILogger<DataService> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task AddBoardAsync(Board board)
    {
        _logger.LogDebug("Added board with Id {Id}", board.Id);
        await _repository.AddAsync(board);
    }

    public async Task<Board?> GetBoardAsync(Ulid boardId)
    {
        _logger.LogDebug("Found board with Id {Id}", boardId);
        return await _repository.GetByIdAsync<Board>(boardId);
    }

    public IEnumerable<Board> GetBoards()
    {
        var count = 0;

        foreach (var board in _repository.GetAll<Board>())
        {
            yield return board;
            count++;
        }

        _logger.LogDebug("Found {Count} boards", count);
    }

    public async IAsyncEnumerable<Category> GetCategoriesAsync()
    {
        var count = 0;
        await foreach (var category in _repository.GetAllAsync<Category>())
        {
            yield return category;
            count++;
        }

        _logger.LogDebug("Found {Count} boards", count);
    }

    public IEnumerable<Category> GetCategories()
    {
        var count = 0;
        foreach (var category in _repository.GetAll<Category>())
        {
            yield return category;
            count++;
        }

        _logger.LogDebug("Found {Count} categories", count);
    }

    public async Task AddCategoryAsync(Category category)
    {
        _logger.LogDebug("Added category with Id {Id}", category.Id);
        await _repository.AddAsync(category);
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        _logger.LogDebug("Deleting category with Id {Id}", category.Id);
        await _repository.DeleteAsync<Category>(category.Id);
    }

    // public async Task<Attachment> UploadAttachmentAsync(
    //     Card card,
    //     string attachmentName,
    //     byte[] attachmentBytes
    // )
    // {
    //     using var stream = new MemoryStream(attachmentBytes);
    //     return await UploadAttachmentAsync(card, attachmentName, stream);
    // }

    // public async Task<Attachment> UploadAttachmentAsync(
    //     Card card,
    //     string attachmentName,
    //     Stream attachmentStream
    // )
    // {
    //     var entryInfo = FileDb.Store(FileDbPath, attachmentName, attachmentStream);
    //
    //     var attachment = new Attachment(
    //         entryInfo.FileName,
    //         entryInfo.FileLength,
    //         entryInfo.MimeType
    //     )
    //     {
    //         Id = new Ulid(entryInfo.ID),
    //         CardId = card.Id,
    //     };
    //
    //     var sqlBuilder = SimpleBuilder
    //         .CreateFluent()
    //         .InsertInto($"Attachment")
    //         .Columns($"Id")
    //         .Columns($"CardId")
    //         .Columns($"Name")
    //         .Columns($"Size")
    //         .Columns($"MimeType")
    //         .Values($"{attachment.Id}")
    //         .Values($"{attachment.CardId}")
    //         .Values($"{attachment.Name}")
    //         .Values($"{attachment.Size}")
    //         .Values($"{attachment.MimeType}");
    //
    //     using var db = await _connectionFactory.CreateAsync();
    //     _logger.LogDebug("Inserting Attachment: {Sql}", sqlBuilder.Sql);
    //     await db.ExecuteAsync(sqlBuilder.Sql, sqlBuilder.Parameters);
    //     return attachment;
    // }
    //
    // public static ValueTask DownloadAttachmentAsync(
    //     Attachment attachment,
    //     string? destinationDirectory = null
    // )
    // {
    //     destinationDirectory ??= EnvironmentHelper.DownloadsDirectory;
    //     var attachmentPath = destinationDirectory.JoinPath(attachment.Name);
    //     FileDb.Read(FileDbPath, attachment.Id.ToGuid(), attachmentPath);
    //     return ValueTask.CompletedTask;
    // }
    //
    // public async Task<IEnumerable<Attachment>> GetAttachmentsAsync(Card card)
    // {
    //     var sqlBuilder = SimpleBuilder
    //         .CreateFluent()
    //         .Select($"*")
    //         .From($"Attachment")
    //         .Where($"CardId = {card.Id}");
    //
    //     using var db = await _connectionFactory.CreateAsync();
    //     _logger.LogDebug("Querying Attachments: {Sql}", sqlBuilder.Sql);
    //     var results = await db.QueryAsync<Attachment>(sqlBuilder.Sql, sqlBuilder.Parameters);
    //     return results;
    // }
    //
    // public async Task UpdateAttachmentAsync(Attachment attachment)
    // {
    //     var sqlBuilder = SimpleBuilder
    //         .CreateFluent()
    //         .Update($"Attachment")
    //         .Set($"Name = {attachment.Name}")
    //         .Set($"Size = {attachment.Size}")
    //         .Set($"MimeType = {attachment.MimeType}")
    //         .Where($"Id = {attachment.Id}");
    //
    //     using var db = await _connectionFactory.CreateAsync();
    //     _logger.LogDebug("Updating Attachment: {Sql}", sqlBuilder.Sql);
    //     await db.ExecuteAsync(sqlBuilder.Sql, sqlBuilder.Parameters);
    // }
    //
    // public async Task<bool> DeleteAttachmentAsync(Attachment attachment)
    // {
    //     var deleted = FileDb.Delete(FileDbPath, attachment.Id.ToGuid());
    //     FileDb.Shrink(FileDbPath);
    //
    //     var sqlBuilder = SimpleBuilder
    //         .CreateFluent()
    //         .DeleteFrom($"Attachment")
    //         .Where($"Id = {attachment.Id}");
    //
    //     using var db = await _connectionFactory.CreateAsync();
    //     _logger.LogDebug("Deleting Attachment: {Sql}", sqlBuilder.Sql);
    //     await db.ExecuteAsync(sqlBuilder.Sql, sqlBuilder.Parameters);
    //
    //     return deleted;
    // }
}
