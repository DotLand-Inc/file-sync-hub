using Dotland.FileSyncHub.Application;
using Dotland.FileSyncHub.Application.Common.Settings;
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
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", service = "ged-backend" })
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
