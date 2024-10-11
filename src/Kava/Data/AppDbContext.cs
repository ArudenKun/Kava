using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kava.Data.Configurations;
using Kava.Data.Converters;
using Kava.Models;
using Kava.Models.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kava.Data;

public sealed class AppDbContext : DbContext
{
    [RequiresUnreferencedCode("Used DbContextOptions")]
    [RequiresDynamicCode("Used DbContextOptions")]
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Ulid>().HaveConversion<UlidToStringConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new BoardConfiguration().Configure(modelBuilder.Entity<Board>());
        new CategoryConfiguration().Configure(modelBuilder.Entity<Category>());
        new CardConfiguration().Configure(modelBuilder.Entity<Card>());
        new AttachmentConfiguration().Configure(modelBuilder.Entity<Attachment>());
    }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AddTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new()
    )
    {
        AddTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker
            .Entries()
            .Where(x =>
                x is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified }
            );

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;

            if (entity.State == EntityState.Added)
            {
                ((BaseEntity)entity.Entity).CreatedAt = now;
            }

            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }
    }
}
