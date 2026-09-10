using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Defra.PTS.Checker.Web.Api.Middleware;

/// <summary>
/// Middleware that catches unhandled exceptions and returns a standardised error response.
/// </summary>
[ExcludeFromCodeCoverage]
public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;

    /// <summary>
    /// Initialises a new instance of the <see cref="ExceptionHandler"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logger">The logger.</param>
    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware for the current request, handling any unhandled exceptions.
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var exceptionModel = new ExceptionModel
        {
            Title = "Internal server error",
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ApplicationException ex:
                if (ex.Message.Contains("Invalid Token"))
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    exceptionModel.Status = (int)HttpStatusCode.Forbidden;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    exceptionModel.Status = (int)HttpStatusCode.BadRequest;
                }

                exceptionModel.Error = ex.Message;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                exceptionModel.Status = (int)HttpStatusCode.InternalServerError;
                exceptionModel.Error = exception.Message;
                break;
        }

        _logger.LogError(exception, exception.Message);

        var result = JsonConvert.SerializeObject(exceptionModel);
        await context.Response.WriteAsync(result, context.RequestAborted);
    }
}
