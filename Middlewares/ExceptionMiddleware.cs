using System.Net;
using System.Text.Json;

namespace ApiElecateProspectsForm.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            (string message, string error, int statusCode) response = (
                message: "An unexpected error occurred. Please try again later.",
                error: exception.Message, // It can be omitted in production to avoid exposing details
                statusCode: (int)HttpStatusCode.InternalServerError
            );

            string jsonResponse = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
