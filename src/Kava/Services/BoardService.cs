using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kava.Core.Data;
using Kava.Core.Helpers;
using Kava.Core.Models.Entities;
using Kava.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kava.Services;

public class BoardService : ISingletonService
{
    private static readonly string FileDbPath = EnvironmentHelper.AppDataDirectory.JoinPath(
        "files.db"
    );

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BoardService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task AddBoardAsync(Board board)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.AddAsync(board);
        await db.SaveChangesAsync();
    }

    public async Task<Board?> GetBoardAsync(Ulid? boardId)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.FindAsync<Board>(boardId);
    }

    public async Task<IReadOnlyList<Attachment>> GetBoardsAsync()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Attachments.AsQueryable().ToListAsync();
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
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entryInfo = FileDb.Store(FileDbPath, attachmentName, attachmentStream);
        var attachment = new Attachment
        {
            Id = new Ulid(entryInfo.ID),
            CardId = card.Id,
            Name = entryInfo.FileName,
            Size = entryInfo.FileLength,
            MimeType = entryInfo.MimeType,
        };

        await db.Attachments.AddAsync(attachment);
        await db.SaveChangesAsync();

        return attachment;
    }

    public ValueTask DownloadAttachmentAsync(
        Attachment attachment,
        string? destinationDirectory = null
    )
    {
        destinationDirectory ??= EnvironmentHelper.DownloadsDirectory;
        var attachmentPath = destinationDirectory.JoinPath(attachment.Name);
        FileDb.Read(FileDbPath, attachment.Id.ToGuid(), attachmentPath);
        return ValueTask.CompletedTask;
    }

    public async Task<IReadOnlyList<Attachment>> GetAttachmentsAsync(Card card)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Attachments.AsQueryable().Where(x => x.CardId == card.Id).ToListAsync();
    }

    public async Task UpdateAttachmentAsync(Attachment attachment)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Update(attachment);
        await db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAttachmentAsync(Attachment attachment)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deleted = FileDb.Delete(FileDbPath, attachment.Id.ToGuid());
        FileDb.Shrink(FileDbPath);

        db.Attachments.Remove(attachment);
        await db.SaveChangesAsync();

        return deleted;
    }
}
