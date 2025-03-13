using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiElecateProspectsForm.Utils
{
    public class ResponseHandler : IResponseHandler
    {
        public IActionResult HandleSuccess(string message)
        {
            return CreateResponse(new StringSuccessMessageResponseDTO
            {
                Status = HttpStatusCode.OK,
                Title = "Success",
                Message = message
            }, HttpStatusCode.OK);
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
                return CreateResponse(new FormFieldErrorMessagesResponseDTO
                {
                    Status = statusCode,
                    Title = GetTitle(statusCode),
                    Errors = errors
                }, statusCode);
            }

            return CreateResponse(new StringErrorMessageResponseDTO
            {
                Status = statusCode,
                Title = GetTitle(statusCode),
                Message = message
            }, statusCode);
        }

        private static IActionResult CreateResponse(object response, HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.BadRequest
                ? new BadRequestObjectResult(response)
                : new ObjectResult(response) { StatusCode = (int)statusCode };
        }

        private static string GetTitle(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.BadRequest ? "Bad Request" : "Internal Server Error";
        }
    }
}