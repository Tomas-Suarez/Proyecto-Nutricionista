using System.Net;
using backend.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace backend.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{

    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly Dictionary<Type, HttpStatusCode> _exceptionStatusMap;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;

        _exceptionStatusMap = new Dictionary<Type, HttpStatusCode>
        {
            { typeof(ResourceNotFoundException), HttpStatusCode.NotFound },
            { typeof(DuplicateResourceException), HttpStatusCode.Conflict },
            { typeof(InvalidCredentialsException), HttpStatusCode.Unauthorized },
            { typeof(SessionExpiredException), HttpStatusCode.Unauthorized },
            { typeof(BadRequestException), HttpStatusCode.BadRequest},
            { typeof(AccessDeniedException), HttpStatusCode.Forbidden},
            { typeof(UnauthenticatedException), HttpStatusCode.Unauthorized}
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = _exceptionStatusMap.GetValueOrDefault(exception.GetType(), HttpStatusCode.InternalServerError);

        if ((int)status >= 500)
        {
            _logger.LogError(exception, "Internal server error [{Type}] - {Message} | Path: {Path}", 
                exception.GetType().Name, exception.Message, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning("Client error [{Type}] - {Message} | Path: {Path}", 
                exception.GetType().Name, exception.Message, httpContext.Request.Path);
        }

        var problemDetail = new ProblemDetails
        {
            Status = (int)status,
            Title = status.ToString(),
            Detail = exception.Message,
            Type = $"https://http.cat/status/{(int)status}",
            Instance = httpContext.Request.Path
        };

        problemDetail.Extensions.Add("timestamp", DateTime.UtcNow);

        httpContext.Response.StatusCode = (int)status;

        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);

        return true;
    }
}
