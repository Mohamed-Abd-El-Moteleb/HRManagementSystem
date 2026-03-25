namespace HRManagementSystem.API.Middleware;
using HRManagementSystem.Domain.Exceptions;
using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "Internal Server Error. Please try again later.";

        if (exception is BaseException baseEx)
        {
            statusCode = baseEx.StatusCode;
            message = baseEx.Message;
        }
        else if (exception is ArgumentException || exception is ArgumentNullException)
        {
            statusCode = (int)HttpStatusCode.BadRequest; // 400
            message = exception.Message;
        }
        context.Response.StatusCode = statusCode;

        var response = new
        {
            Status = statusCode,
            Message = message,
            Details = _env.IsDevelopment() ? exception.StackTrace?.ToString() : null
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }

}
