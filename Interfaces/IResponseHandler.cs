using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IResponseHandler
    {
        IActionResult HandleSuccess(string message);

        IActionResult HandleError(string message,
           HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
           Exception? ex = null, bool isArrayErrorMessage = false, List<FieldErrorDTO>? errors = null);
    }
}