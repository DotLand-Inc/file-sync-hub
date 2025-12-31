using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dotland.FileSyncHub.Application.Common.Behaviours;

/// <summary>
/// Performance monitoring behavior for MediatR pipeline.
/// Logs warnings for requests that take longer than 500ms.
/// </summary>
public class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                requestName, elapsedMilliseconds, request);
        }

        return response;
    }
}
