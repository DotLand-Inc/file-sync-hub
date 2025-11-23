using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Configurations;

public class OrganizationVersioningConfigurationConfiguration : IEntityTypeConfiguration<OrganizationVersioningConfiguration>
{
    public void Configure(EntityTypeBuilder<OrganizationVersioningConfiguration> builder)
    {
        builder.ToTable("OrganizationVersioningConfigurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.OrganizationId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(200);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(200);

        builder.HasIndex(c => c.OrganizationId).IsUnique();
        builder.HasIndex(c => c.IsActive);

        builder.HasMany(c => c.CategoryConfigurations)
            .WithOne(cc => cc.OrganizationConfiguration)
            .HasForeignKey(cc => cc.OrganizationVersioningConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.CategoryConfigurations).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CategoryVersioningConfigurationConfiguration : IEntityTypeConfiguration<CategoryVersioningConfiguration>
{
    public void Configure(EntityTypeBuilder<CategoryVersioningConfiguration> builder)
    {
        builder.ToTable("CategoryVersioningConfigurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(200);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(200);

        builder.HasIndex(c => new { c.OrganizationVersioningConfigurationId, c.Category }).IsUnique();
    }
}
