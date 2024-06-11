using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace Defra.PTS.Checker.Web.Api.Middleware;

[ExcludeFromCodeCoverage]
public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

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

        var exceptionModel = new ExceptionModel();
        
        switch (exception)
        {
            case ApplicationException ex:
                if (ex.Message.Contains("Invalid Token"))
                {
                    response.StatusCode = exceptionModel.StatusCode = (int)HttpStatusCode.Forbidden;
                    exceptionModel.StatusCode = (int)HttpStatusCode.Forbidden;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    exceptionModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                exceptionModel.Message = ex.Message;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                exceptionModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                exceptionModel.Message = $"Internal server error: {exception.Message}";
                break;
        }

        _logger.LogError(exception, exception.Message);
        
        var result = JsonSerializer.Serialize(exceptionModel);
        await context.Response.WriteAsync(result);
    }
}
