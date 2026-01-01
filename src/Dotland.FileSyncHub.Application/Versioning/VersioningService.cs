using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Entities;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Domain.Repositories;

namespace Dotland.FileSyncHub.Application.Versioning;

/// <summary>
/// Service implementation for managing versioning configurations
/// </summary>
public class VersioningService : IVersioningService
{
    private readonly IUnitOfWork _unitOfWork;

    public VersioningService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrganizationVersioningConfigurationDto?> GetOrganizationConfigurationAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        var config = await _unitOfWork.VersioningConfigurations.GetByOrganizationIdWithCategoriesAsync(organizationId, cancellationToken);

        if (config == null)
            return null;

        return MapToDto(config);
    }

    public async Task<IReadOnlyList<OrganizationVersioningConfigurationDto>> GetAllConfigurationsAsync(CancellationToken cancellationToken = default)
    {
        var configs = await _unitOfWork.VersioningConfigurations.GetAllActiveAsync(cancellationToken);
        return configs.Select(MapToDto).ToList();
    }

    public async Task<OrganizationVersioningConfigurationDto> CreateOrganizationConfigurationAsync(CreateOrganizationVersioningConfigurationDto dto, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var existingConfig = await _unitOfWork.VersioningConfigurations.GetByOrganizationIdAsync(dto.OrganizationId, cancellationToken);

        if (existingConfig != null)
            throw new InvalidOperationException($"Configuration already exists for organization {dto.OrganizationId}");

        var config = OrganizationVersioningConfiguration.Create(
            dto.OrganizationId,
            dto.DefaultVersioningEnabled,
            dto.DefaultMaxVersions,
            createdBy
        );

        await _unitOfWork.VersioningConfigurations.AddAsync(config, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(config);
    }

    public async Task<OrganizationVersioningConfigurationDto> UpdateOrganizationConfigurationAsync(string organizationId, UpdateOrganizationVersioningConfigurationDto dto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var config = await _unitOfWork.VersioningConfigurations.GetByOrganizationIdWithCategoriesAsync(organizationId, cancellationToken);

        if (config == null)
            throw new InvalidOperationException($"Configuration not found for organization {organizationId}");

        config.Update(dto.DefaultVersioningEnabled, dto.DefaultMaxVersions, updatedBy);

        await _unitOfWork.VersioningConfigurations.UpdateAsync(config, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(config);
    }

    public async Task RemoveCategoryConfigurationAsync(string organizationId, DocumentCategory category, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var config = await _unitOfWork.VersioningConfigurations.GetByOrganizationIdWithCategoriesAsync(organizationId, cancellationToken);

        if (config == null)
            throw new InvalidOperationException($"Configuration not found for organization {organizationId}");

        config.RemoveCategoryConfiguration(category);
        config.SetUpdated(updatedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.VersioningConfigurations.IsVersioningEnabledAsync(organizationId, category, cancellationToken);
    }

    public async Task<int> GetMaxVersionsAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.VersioningConfigurations.GetMaxVersionsAsync(organizationId, category, cancellationToken);
    }

    public async Task DeleteOrganizationConfigurationAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        var config = await _unitOfWork.VersioningConfigurations.GetByOrganizationIdAsync(organizationId, cancellationToken);

        if (config == null)
            throw new InvalidOperationException($"Configuration not found for organization {organizationId}");

        config.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static OrganizationVersioningConfigurationDto MapToDto(OrganizationVersioningConfiguration config)
    {
        return new OrganizationVersioningConfigurationDto(
            config.Id,
            config.OrganizationId,
            config.DefaultVersioningEnabled,
            config.DefaultMaxVersions,
            config.IsActive,
            config.CategoryConfigurations.Select(c => new CategoryVersioningConfigurationDto(
                c.Id,
                c.Category,
                c.VersioningEnabled,
                c.MaxVersions
            )).ToList()
        );
    }
}
