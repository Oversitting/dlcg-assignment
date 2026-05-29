using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Api.Implementation.Exceptions;

namespace VideoGameCatalogue.Api.Implementation.ErrorHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private const int ClientClosedRequestStatusCode = 499;

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            return false;
        }

        var error = MapException(exception, httpContext);

        _logger.Log(
            error.LogLevel,
            exception,
            "Request {Method} {Path} failed with status code {StatusCode}. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            error.StatusCode,
            httpContext.TraceIdentifier);

        IResult result = exception is ValidationException validationException
            ? Results.ValidationProblem(
                errors: validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).Distinct().ToArray()),
                statusCode: error.StatusCode,
                title: error.Title,
                type: error.Type,
                detail: error.Detail,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = httpContext.TraceIdentifier
                })
            : Results.Problem(
                statusCode: error.StatusCode,
                title: error.Title,
                detail: error.Detail,
                type: error.Type,
                instance: httpContext.Request.Path,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = httpContext.TraceIdentifier
                });

        await result.ExecuteAsync(httpContext);
        return true;
    }

    private static ErrorDescriptor MapException(Exception exception, HttpContext httpContext)
    {
        return exception switch
        {
            ValidationException validationException => new ErrorDescriptor(
                StatusCodes.Status400BadRequest,
                "Request validation failed.",
                validationException.Message,
                "https://httpstatuses.com/400",
                LogLevel.Warning),

            ResourceNotFoundException notFound => new ErrorDescriptor(
                StatusCodes.Status404NotFound,
                "Resource not found.",
                notFound.Message,
                "https://httpstatuses.com/404",
                LogLevel.Warning),

            ArgumentOutOfRangeException argumentOutOfRange => new ErrorDescriptor(
                StatusCodes.Status400BadRequest,
                "Invalid request data.",
                argumentOutOfRange.Message,
                "https://httpstatuses.com/400",
                LogLevel.Warning),

            ArgumentException argumentException => new ErrorDescriptor(
                StatusCodes.Status400BadRequest,
                "Invalid request data.",
                argumentException.Message,
                "https://httpstatuses.com/400",
                LogLevel.Warning),

            BadHttpRequestException badHttpRequest => new ErrorDescriptor(
                StatusCodes.Status400BadRequest,
                "Malformed request.",
                badHttpRequest.Message,
                "https://httpstatuses.com/400",
                LogLevel.Warning),

            DbUpdateException => new ErrorDescriptor(
                StatusCodes.Status503ServiceUnavailable,
                "A database error occurred.",
                "The catalogue database operation failed. Retry the request after verifying the database connection.",
                "https://httpstatuses.com/503",
                LogLevel.Error),

            OperationCanceledException when httpContext.RequestAborted.IsCancellationRequested => new ErrorDescriptor(
                ClientClosedRequestStatusCode,
                "Request cancelled.",
                "The request was cancelled before completion.",
                "about:blank",
                LogLevel.Information),

            OperationCanceledException => new ErrorDescriptor(
                StatusCodes.Status408RequestTimeout,
                "Request timed out.",
                "The operation did not complete before it was cancelled.",
                "https://httpstatuses.com/408",
                LogLevel.Warning),

            _ => new ErrorDescriptor(
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "The server encountered an unexpected condition while processing the request.",
                "https://httpstatuses.com/500",
                LogLevel.Error)
        };
    }

    private readonly record struct ErrorDescriptor(
        int StatusCode,
        string Title,
        string Detail,
        string Type,
        LogLevel LogLevel);
}