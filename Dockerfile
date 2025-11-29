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
RUN dotnet restore

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

# Create non-root user
RUN addgroup -g 1000 appuser && \
    adduser -u 1000 -G appuser -s /bin/sh -D appuser

# Install curl for healthchecks
RUN apk add --no-cache curl

# Copy published app
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Dotland.FileSyncHub.Web.dll"]
