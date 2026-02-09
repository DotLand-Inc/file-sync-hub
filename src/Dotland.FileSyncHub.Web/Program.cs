using Dotland.FileSyncHub.Application;
using Dotland.FileSyncHub.Application.Common.Settings;
using Dotland.FileSyncHub.Domain.Common.Exceptions;
using Dotland.FileSyncHub.Infrastructure;
using Dotland.FileSyncHub.Infrastructure.Persistence;
using Dotland.FileSyncHub.Web.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configuration - Environment variables override appsettings.json
// Use S3__BucketName, S3__Region, S3__ServiceUrl format for env vars
builder.Services.Configure<S3Settings>(builder.Configuration.GetSection(S3Settings.SectionName));

// Clean Architecture layers
builder.Services.AddWebServices(builder.Configuration);

// Exception Handling
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

// Controllers
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddOpenApi();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var authority = builder.Configuration["Keycloak:Authority"];
        if(string.IsNullOrEmpty(authority)) throw new ConfigurationKeyException("Keycloak:Authority");
        
        var audience = builder.Configuration["Keycloak:Audience"];
        if(string.IsNullOrEmpty(audience)) throw new ConfigurationKeyException("Keycloak:Audience");
        
        var metadata = builder.Configuration["Keycloak:MetadataAddress"];
        if(string.IsNullOrEmpty(metadata)) throw new ConfigurationKeyException("Keycloak:MetadataAddress");
        
        options.Authority = authority;
        options.Audience = audience;
        options.MetadataAddress = metadata;
        options.RequireHttpsMetadata = bool.Parse(builder.Configuration["Keycloak:RequireHttpsMetadata"] ?? "true");
        
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = authority
        };
    });

var app = builder.Build();

// Apply database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileSyncHubDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
app.UseExceptionHandler();

// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", service = "ged-backend" })
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
