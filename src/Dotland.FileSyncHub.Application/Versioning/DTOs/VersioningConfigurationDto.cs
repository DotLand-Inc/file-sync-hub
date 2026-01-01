using Dotland.FileSyncHub.Domain.Enums;
using System.Text.Json.Serialization;

namespace Dotland.FileSyncHub.Application.Versioning.DTOs;

public record OrganizationVersioningConfigurationDto(
    Guid Id,
    string OrganizationId,
    bool DefaultVersioningEnabled,
    int DefaultMaxVersions,
    bool IsActive,
    List<CategoryVersioningConfigurationDto> CategoryConfigurations
);

public record CategoryVersioningConfigurationDto(
    Guid Id,
    DocumentCategory Category,
    bool VersioningEnabled,
    int MaxVersions
);

public record CreateOrganizationVersioningConfigurationDto(
    string OrganizationId,
    bool DefaultVersioningEnabled,
    int DefaultMaxVersions
);

public record UpdateOrganizationVersioningConfigurationDto(
    bool DefaultVersioningEnabled,
    int DefaultMaxVersions
);

public record SetCategoryVersioningConfigurationDto(
    [property: JsonConverter(typeof(Dotland.FileSyncHub.Application.Common.Converters.DocumentCategoryJsonConverter))]
    DocumentCategory Category,
    bool VersioningEnabled,
    int MaxVersions
);
