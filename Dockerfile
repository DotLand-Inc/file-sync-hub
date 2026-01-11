# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copy solution and project files
COPY file-sync-hub.sln .
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY src/Dotland.FileSyncHub.Domain/*.csproj ./src/Dotland.FileSyncHub.Domain/
COPY src/Dotland.FileSyncHub.Application/*.csproj ./src/Dotland.FileSyncHub.Application/
COPY src/Dotland.FileSyncHub.Infrastructure/*.csproj ./src/Dotland.FileSyncHub.Infrastructure/
COPY src/Dotland.FileSyncHub.Web/*.csproj ./src/Dotland.FileSyncHub.Web/

# Restore dependencies
RUN dotnet restore file-sync-hub.sln

# Copy source code
COPY src/ ./src/

# Build and publish
RUN dotnet publish src/Dotland.FileSyncHub.Web/Dotland.FileSyncHub.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Install curl for healthchecks
RUN apk add --no-cache curl

# Copy published app
COPY --from=build /app/publish .


# # Switch to non-root user
# USER appuser

# Environment variables
# ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:5000 \
    AllowedHosts=* \
    # Database
    UseInMemoryDatabase=false \
    ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=filesyncdb;Username=postgres;Password=changeme" \
    # S3 Configuration
    S3__Region=eu-west-1 \
    S3__BucketName=filesync-bucket \
    S3__ServiceUrl="" \
    S3__MaxFileSizeMb=100 \
    # AWS Credentials (should be overridden at runtime)
    AWS_ACCESS_KEY_ID="" \
    AWS_SECRET_ACCESS_KEY="" \
    # Logging
    Logging__LogLevel__Default=Information \
    Logging__LogLevel__Microsoft.AspNetCore=Warning \
    Logging__LogLevel__Microsoft.EntityFrameworkCore.Database.Command=Information

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Dotland.FileSyncHub.Web.dll"]
