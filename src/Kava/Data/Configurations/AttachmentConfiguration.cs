using Kava.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kava.Data.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.MimeType).HasMaxLength(25);

        builder
            .HasOne(x => x.Card)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.CardId)
            .IsRequired();
    }
}
