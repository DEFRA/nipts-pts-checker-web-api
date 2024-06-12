using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;

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
        await context.Response.WriteAsync(result);
    }
}
