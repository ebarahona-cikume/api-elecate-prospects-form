using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiElecateProspectsForm.Utils
{
    public static class ValidateFormFields
    {
        public static IActionResult ValidateFormFilds(IQueryable<FormFieldsModel> formFields, 
            IResponseHandler responseHandler)
        {
            if (formFields == null || !formFields.Any())
            {
                return responseHandler.HandleError("You must provide at least one field", HttpStatusCode.BadRequest);
            }

            List<FieldErrorDTO> errors = [];

            foreach (FormFieldsModel field in formFields)
            {
                List<string> fieldErrors = [];

                if (field.Type == "Text"
                    && field.Size.HasValue
                    && field.Name?.Length > field.Size.Value)
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
                return responseHandler.HandleError("Validation errors occurred", 
                    HttpStatusCode.BadRequest, 
                    isArrayErrorMessage: true, 
                    errors: errors);
            }

            return new OkResult();
        }
    }
}
