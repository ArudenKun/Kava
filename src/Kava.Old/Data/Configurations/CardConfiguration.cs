using Kava.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kava.Data.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(50);

        builder
            .HasOne(x => x.Category)
            .WithMany(x => x.Cards)
            .HasForeignKey(x => x.CategoryId)
            .IsRequired();

        builder
            .HasMany(x => x.Attachments)
            .WithOne(x => x.Card)
            .HasForeignKey(x => x.CardId)
            .IsRequired(false);
    }
}
