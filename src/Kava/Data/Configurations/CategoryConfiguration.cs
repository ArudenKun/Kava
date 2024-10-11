using Kava.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kava.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(500);

        builder
            .HasOne(x => x.Board)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.BoardId)
            .IsRequired();

        builder
            .HasMany(x => x.Cards)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .IsRequired(false);
    }
}
