using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Configurations;

public class DocumentStatusHistoryConfiguration : IEntityTypeConfiguration<DocumentStatusHistory>
{
    public void Configure(EntityTypeBuilder<DocumentStatusHistory> builder)
    {
        builder.ToTable("DocumentStatusHistory");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Comment)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(h => h.ChangedBy)
            .HasMaxLength(200);

        builder.Property(h => h.CreatedBy)
            .HasMaxLength(200);

        builder.Property(h => h.UpdatedBy)
            .HasMaxLength(200);

        builder.HasIndex(h => h.DocumentId);
        builder.HasIndex(h => new { h.DocumentId, h.CreatedAt });
    }
}
