using System.Diagnostics.CodeAnalysis;
using Kava.Core.Models;
using Kava.Data.Configurations;
using Kava.Data.Converters;
using Microsoft.EntityFrameworkCore;

namespace Kava.Data;

[RequiresUnreferencedCode("Calls DbContext Ctor")]
[RequiresDynamicCode("Calls DbContext Ctor")]
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
        configurationBuilder.Properties<Ulid>().HaveConversion<UlidToStringConverter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new BoardConfiguration().Configure(modelBuilder.Entity<Board>());
        new CategoryConfiguration().Configure(modelBuilder.Entity<Category>());
        new CardConfiguration().Configure(modelBuilder.Entity<Card>());
        new AttachmentConfiguration().Configure(modelBuilder.Entity<Attachment>());
    }
}
