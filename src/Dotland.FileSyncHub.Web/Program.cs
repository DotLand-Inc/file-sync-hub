using Amazon;
using Amazon.S3;
using Dotland.FileSyncHub.Web.Configuration;
using Dotland.FileSyncHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration - Environment variables override appsettings.json
// Use S3__BucketName, S3__Region, S3__ServiceUrl format for env vars
builder.Services.Configure<S3Settings>(builder.Configuration.GetSection(S3Settings.SectionName));

// AWS S3
var s3Settings = builder.Configuration.GetSection(S3Settings.SectionName).Get<S3Settings>() ?? new S3Settings();

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
    };

    // Support for LocalStack or S3-compatible services
    if (!string.IsNullOrEmpty(s3Settings.ServiceUrl))
    {
        config.ServiceURL = s3Settings.ServiceUrl;
        config.ForcePathStyle = true;
    }

    // AWS SDK automatically reads credentials from:
    // 1. Environment variables: AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY
    // 2. AWS credentials file: ~/.aws/credentials
    // 3. IAM roles (EC2/ECS/Lambda)
    return new AmazonS3Client(config);
});

// Services
builder.Services.AddScoped<IS3StorageService, S3StorageService>();

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

// Configure the HTTP request pipeline
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
