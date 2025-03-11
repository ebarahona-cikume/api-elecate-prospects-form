using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiElecateProspectsForm.Utils
{
    public class ResponseHandler
    {
        public IActionResult HandleSuccess(string message)
        {
            return new OkObjectResult(new StringSuccessMessageResponseDTO
            {
                Status = HttpStatusCode.OK,
                Title = "Success",
                Message = message
            });
        }

        public IActionResult HandleError(string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            Exception? ex = null, bool isArrayErrorMessage = false, List<FieldErrorDTO>? errors = null)
        {
            if (isArrayErrorMessage && errors != null)
            {
                var arrayErrorResponse = new ArrayErrorMessageResponseDTO
                {
                    Status = statusCode,
                    Title = statusCode == HttpStatusCode.BadRequest ? "Bad Request" : "Internal Server Error",
                    Errors = errors
                };

                if (statusCode == HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult(arrayErrorResponse);
                }

                return new ObjectResult(arrayErrorResponse)
                {
                    StatusCode = (int)statusCode
                };
            }
            else
            {
                var errorResponse = new StringErrorMessageResponseDTO
                {
                    Status = statusCode,
                    Title = statusCode == HttpStatusCode.BadRequest ? "Bad Request" : "Internal Server Error",
                    Message = message // Use the provided message instead of ex.Message
                };

                if (statusCode == HttpStatusCode.BadRequest)
                {
                    return new BadRequestObjectResult(errorResponse);
                }

                return new ObjectResult(errorResponse)
                {
                    StatusCode = (int)statusCode
                };
            }
        }

    }
}
