using Dotland.FileSyncHub.Domain.Repositories;
using Dotland.FileSyncHub.Infrastructure.Persistence;
using Dotland.FileSyncHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dotland.FileSyncHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase");

        if (useInMemory || string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<FileSyncHubDbContext>(options =>
                options.UseInMemoryDatabase("FileSyncHubDb"));
        }
        else
        {
            services.AddDbContext<FileSyncHubDbContext>(options =>
                options.UseSqlite(connectionString));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentVersionRepository, DocumentVersionRepository>();
        services.AddScoped<IVersioningConfigurationRepository, VersioningConfigurationRepository>();

        return services;
    }
}
