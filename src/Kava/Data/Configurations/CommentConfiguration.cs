using Kava.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kava.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(x => x.Content).HasMaxLength(255).IsRequired();
        builder
            .HasOne(x => x.Card)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.CardId)
            .IsRequired();
    }
}
