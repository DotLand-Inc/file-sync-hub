using Dotland.FileSyncHub.Application.Common.Settings;
using Dotland.FileSyncHub.Application.Versioning;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
        });

        services.Configure<S3Settings>(
            configuration.GetSection(S3Settings.SectionName));

        services.AddScoped<IVersioningService, VersioningService>();

        return services;
    }
}
