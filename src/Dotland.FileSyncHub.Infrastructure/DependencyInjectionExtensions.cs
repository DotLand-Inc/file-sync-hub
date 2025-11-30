using Amazon;
using Amazon.S3;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Common.Settings;
using Dotland.FileSyncHub.Domain.Repositories;
using Dotland.FileSyncHub.Infrastructure.Persistence;
using Dotland.FileSyncHub.Infrastructure.Persistence.Repositories;
using Dotland.FileSyncHub.Infrastructure.ThirdParty.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // AWS S3 Client
        var s3Settings = configuration.GetSection(S3Settings.SectionName).Get<S3Settings>() ?? new S3Settings();
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
            };

            // Support for LocalStack or S3-compatible services
            if (string.IsNullOrEmpty(s3Settings.ServiceUrl)) return new AmazonS3Client(config);
            
            config.ServiceURL = s3Settings.ServiceUrl;
            config.ForcePathStyle = true;

            // AWS SDK automatically reads credentials from:
            // 1. Environment variables: AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY
            // 2. AWS credentials file: ~/.aws/credentials
            // 3. IAM roles (EC2/ECS/Lambda)
            return new AmazonS3Client(config);
        });

        // PostgreSQL Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string is required. " +
                "Please configure 'ConnectionStrings:DefaultConnection' in appsettings.json");
        }

        services.AddDbContext<FileSyncHubDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(FileSyncHubDbContext).Assembly.GetName().Name);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });

            // Enable sensitive data logging in development
            if (bool.TryParse(configuration["Logging:EnableSensitiveDataLogging"], out var enableSensitiveLogging) && enableSensitiveLogging)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentVersionRepository, DocumentVersionRepository>();
        services.AddScoped<IVersioningConfigurationRepository, VersioningConfigurationRepository>();

        services.AddScoped<IS3StorageService, S3StorageService>();

        return services;
    }
}
