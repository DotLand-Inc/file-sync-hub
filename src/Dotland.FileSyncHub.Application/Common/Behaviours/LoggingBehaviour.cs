using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Dotland.FileSyncHub.Application.Common.Behaviours;

/// <summary>
/// Logging behavior for MediatR pipeline.
/// Logs incoming requests with contextual information.
/// </summary>
public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Request: {Name} {@Request}",
            requestName, request);

        return Task.CompletedTask;
    }
}
