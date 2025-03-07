using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiElecateProspectsForm.Utils
{
    public static class ValidateFormFields
    {
        public static IActionResult ValidateFormFilds(IQueryable<FormFieldsModel> formFields)
        {
            if (formFields == null || !formFields.Any())
            {
                StringErrorMessageResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Message = "You must provide at least one field"
                };

                return new BadRequestObjectResult(errorResponse);
            }

            List<FieldErrorDTO> errors = [];

            foreach (var field in formFields)
            {
                List<string> fieldErrors = [];

                if (field.Type == "Text" && field.Size.HasValue && field.Name?.Length > field.Size.Value)
                {
                    fieldErrors.Add($"The field '{field.Name}' exceeds the maximum size of {field.Size.Value} characters.");
                }

                if (fieldErrors.Count > 0)
                {
                    errors.Add(new FieldErrorDTO
                    {
                        Index = field.Id,
                        FieldErrors = fieldErrors
                    });
                }
            }

            if (errors.Count > 0)
            {
                ArrayErrorMessageResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            }

            return new OkResult();
        }
    }
}
