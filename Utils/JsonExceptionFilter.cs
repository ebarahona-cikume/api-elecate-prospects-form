using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net;

namespace ApiElecateProspectsForm.Utils
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Verifica si la excepción es una JsonException
            if (context.Exception is JsonException jsonException)
            {
                // Personaliza la respuesta de error para deserialización
                var errorResponse = new ErrorResponseDTO
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Message = "There was an error processing the request body."
                };

                context.Result = new BadRequestObjectResult(errorResponse);

                // Evita que ASP.NET maneje la excepción
                context.ExceptionHandled = true;
            }
        }
    }
}
