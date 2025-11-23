using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.OrganizationId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.Description)
            .HasMaxLength(2000);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.S3Key)
            .HasMaxLength(1000);

        builder.Property(d => d.WorkflowInstanceId)
            .HasMaxLength(100);

        builder.Property(d => d.CreatedBy)
            .HasMaxLength(200);

        builder.Property(d => d.UpdatedBy)
            .HasMaxLength(200);

        builder.HasIndex(d => d.OrganizationId);
        builder.HasIndex(d => d.Category);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.WorkflowInstanceId);
        builder.HasIndex(d => new { d.OrganizationId, d.Category });

        builder.HasMany(d => d.Versions)
            .WithOne(v => v.Document)
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.StatusHistory)
            .WithOne(h => h.Document)
            .HasForeignKey(h => h.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(d => d.Versions).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(d => d.StatusHistory).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
