using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Configurations;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.AwsVersionId)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(v => v.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Comment)
            .HasMaxLength(1000);

        builder.Property(v => v.CreatedBy)
            .HasMaxLength(200);

        builder.Property(v => v.UpdatedBy)
            .HasMaxLength(200);

        builder.HasIndex(v => v.DocumentId);
        builder.HasIndex(v => new { v.DocumentId, v.VersionNumber }).IsUnique();
    }
}
