using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kava.Data;
using Kava.Helpers;
using Kava.Models;
using Kava.Services.Abstractions;
using Kava.Services.Abstractions.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kava.Services;

public class BoardService : ISingletonService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IFileDbFactory _fileDbFactory;

    public BoardService(IServiceScopeFactory serviceScopeFactory, IFileDbFactory fileDbFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _fileDbFactory = fileDbFactory;
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
        using var fileDb = _fileDbFactory.Create(FileAccess.Write);
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entryInfo = fileDb.Store(attachmentName, attachmentStream);
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
        using var fileDb = _fileDbFactory.Create(FileAccess.Read);
        _ = fileDb.Read(attachment.Id.ToGuid(), attachmentPath);
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
        using var fileDb = _fileDbFactory.Create(FileAccess.Write);
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deleted = fileDb.Delete(attachment.Id.ToGuid());
        fileDb.Shrink();

        db.Attachments.Remove(attachment);
        await db.SaveChangesAsync();

        return deleted;
    }
}
