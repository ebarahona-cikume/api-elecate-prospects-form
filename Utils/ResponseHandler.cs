using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
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

        public IActionResult HandleError(
            string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            Exception? ex = null,
            bool isFieldErrors = false,
            List<FieldErrorDTO>? fieldErrors = null, 
            List<string>? generalErrors = null
        )
        {
            var title = statusCode == HttpStatusCode.BadRequest ? "Bad Request" : "Internal Server Error";

            if (isFieldErrors && fieldErrors != null)
            {
                var fieldErrorsResponse = new FormFieldErrorsMessagesResponseDTO
                {
                    Status = statusCode,
                    Title = title,
                    Errors = fieldErrors
                };

                return new ObjectResult(fieldErrorsResponse) { StatusCode = (int)statusCode };
            }

            if (generalErrors != null)
            {
                var generalErrorsResponse = new GeneralErrorsResponseDTO
                {
                    Status = statusCode,
                    Title = title,
                    Errors = generalErrors
                };

                return new ObjectResult(generalErrorsResponse) { StatusCode = (int)statusCode };
            }

            var errorResponse = new StringErrorMessageResponseDTO
            {
                Status = statusCode,
                Title = title,
                Message = message
            };

            return new ObjectResult(errorResponse) { StatusCode = (int)statusCode };
        }

    }
}
