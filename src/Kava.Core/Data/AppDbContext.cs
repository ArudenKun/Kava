using System.Diagnostics.CodeAnalysis;
using Kava.Core.Data.Configurations;
using Kava.Core.Data.Converters;
using Kava.Core.Data.Interceptors;
using Kava.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kava.Core.Data;

[RequiresUnreferencedCode("Calls DbContext Ctor")]
[RequiresDynamicCode("Calls DbContext Ctor")]
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    private static readonly TimestampInterceptor TimestampInterceptor = new();

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.AddInterceptors(TimestampInterceptor);

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
