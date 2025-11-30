using System.Reflection;
using Dotland.FileSyncHub.Application.Common.Behaviours;
using Dotland.FileSyncHub.Application.Common.Settings;
using Dotland.FileSyncHub.Application.Versioning;
using FluentValidation;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.Configure<S3Settings>(
            configuration.GetSection(S3Settings.SectionName));

        services.AddScoped<IVersioningService, VersioningService>();

        return services;
    }
}
