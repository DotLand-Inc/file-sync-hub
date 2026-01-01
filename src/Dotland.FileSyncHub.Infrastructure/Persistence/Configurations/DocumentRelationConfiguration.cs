using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Configurations;

public class DocumentRelationConfiguration : IEntityTypeConfiguration<DocumentRelation>
{
    public void Configure(EntityTypeBuilder<DocumentRelation> builder)
    {
        builder.ToTable("DocumentRelations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(200);

        builder.Property(r => r.UpdatedBy)
            .HasMaxLength(200);

        // Configure the relationship from SourceDocument to TargetDocument
        builder.HasOne(r => r.SourceDocument)
            .WithMany(d => d.ParentRelations)
            .HasForeignKey(r => r.SourceDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.TargetDocument)
            .WithMany(d => d.ChildRelations)
            .HasForeignKey(r => r.TargetDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ensure a document can only have one parent (unique constraint on SourceDocumentId)
        builder.HasIndex(r => r.SourceDocumentId)
            .IsUnique();

        builder.HasIndex(r => r.TargetDocumentId);
    }
}
