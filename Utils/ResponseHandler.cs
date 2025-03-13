using ApiElecateProspectsForm.DTOs;
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

        public IActionResult HandleError(string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            Exception? ex = null, bool isArrayErrorMessage = false, List<FieldErrorDTO>? errors = null)
        {
            if (isArrayErrorMessage && errors != null)
            {
                return CreateResponse(new ArrayErrorMessageResponseDTO
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

        private IActionResult CreateResponse(object response, HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.BadRequest
                ? new BadRequestObjectResult(response)
                : new ObjectResult(response) { StatusCode = (int)statusCode };
        }

        private string GetTitle(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.BadRequest ? "Bad Request" : "Internal Server Error";
        }
    }
}