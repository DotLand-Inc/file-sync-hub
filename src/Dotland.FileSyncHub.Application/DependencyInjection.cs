using Dotland.FileSyncHub.Application.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Dotland.FileSyncHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddScoped<IVersioningService, VersioningService>();

        return services;
    }
}
