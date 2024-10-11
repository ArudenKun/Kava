using Kava.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kava.Core.Data.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(50);

        builder
            .HasMany(x => x.Categories)
            .WithOne(x => x.Board)
            .HasForeignKey(x => x.BoardId)
            .IsRequired(false);
    }
}
