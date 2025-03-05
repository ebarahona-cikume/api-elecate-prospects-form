using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiElecateProspectsForm.Utils
{
    public static class ValidateFields
    {
        public static IActionResult Validate(GenerateFormRequestDTO request)
        {
            if (request == null || request.Fields == null || request.Fields.Count == 0)
            {
                ErrorResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Message = "You must provide at least one field"
                };

                return new BadRequestObjectResult(errorResponse);
            }

            var errors = new List<string>();

            for (int i = 0; i < request.Fields.Count; i++)
            {
                var originalJson = request.OriginalJsonFields[i];

                if (request.Fields[i] == null)
                {
                    if (!originalJson.ContainsKey("type"))
                    {
                        errors.Add($"Invalid field at index {i}: 'type' is missing.");
                    }
                    else
                    {
                        errors.Add($"Invalid field at index {i}: 'type' value is invalid.");
                    }
                }
            }

            if (errors.Count > 0)
            {
                ErrorResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Message = string.Join(" ", errors)
                };

                return new BadRequestObjectResult(errorResponse);
            }

            foreach (var property in from FormFieldRequestDTO field in request.Fields
                                     let properties = field.GetType().GetProperties()
                                     from System.Reflection.PropertyInfo property in properties
                                     let value = property.GetValue(field) as string
                                     where property.PropertyType == typeof(string) && string.IsNullOrEmpty(value) && property.Name != "Mask"
                                     select property)
            {
                ErrorResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Message = $"The field '{property.Name}' is required"
                };

                return new BadRequestObjectResult(errorResponse);
            }

            return new OkResult();
        }
    }
}