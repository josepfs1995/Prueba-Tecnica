using MediatR;
using Microsoft.Extensions.Logging;

namespace Navisaf.Application.Common.PipelineBehavior;

public class LoggerBehavior<TRequest, TResponse>(ILogger<TRequest> logger): IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next(cancellationToken);
        logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}