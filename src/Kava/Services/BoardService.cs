using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kava.Core.Models;
using Kava.Data;
using Kava.Helpers;
using Kava.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kava.Services;

[RequiresUnreferencedCode("Calls DbContext Ctor")]
public class BoardService : ISingletonService
{
    private static readonly string FileDbPath = EnvironmentHelper.AppDataDirectory.JoinPath(
        "files.db"
    );

    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public BoardService(
        IServiceScopeFactory serviceScopeFactory,
        IDbContextFactory<AppDbContext> dbContextFactory
    )
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddBoardAsync(Board board)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        await db.AddAsync(board);
        await db.SaveChangesAsync();
    }

    public async Task<Board?> GetBoardAsync(Ulid? boardId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.FindAsync<Board>(boardId);
    }

    public async Task<IReadOnlyList<Attachment>> GetBoardsAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
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
        await using var db = await _dbContextFactory.CreateDbContextAsync();
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
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Attachments.AsQueryable().Where(x => x.CardId == card.Id).ToListAsync();
    }

    public async Task UpdateAttachmentAsync(Attachment attachment)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        db.Update(attachment);
        await db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAttachmentAsync(Attachment attachment)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var deleted = FileDb.Delete(FileDbPath, attachment.Id.ToGuid());
        FileDb.Shrink(FileDbPath);

        db.Attachments.Remove(attachment);
        await db.SaveChangesAsync();

        return deleted;
    }
}
