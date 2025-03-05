using ApiElecateProspectsForm.Controllers;
using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ApiElecateProspectsForm.Utils
{
    public static class ValidateFields
    {
        public static IActionResult Validate(FormRequestDTO request)
        {
            if (request == null || request.Fields == null || request.Fields.Count == 0)
            {
                return new BadRequestObjectResult("Debe proporcionar al menos un campo.");
            }

            foreach (var property in from FormFieldRequestDTO field in request.Fields ?? Enumerable.Empty<FormFieldRequestDTO>()
                                     let properties = field.GetType().GetProperties()
                                     from System.Reflection.PropertyInfo property in properties
                                     let value = property.GetValue(field) as string
                                     where property.PropertyType == typeof(string) && string.IsNullOrEmpty(value) && property.Name != "Mask"
                                     select property)
            {
                return new BadRequestObjectResult($"El campo '{property.Name}' es obligatorio.");
            }

            return new OkResult();
        }
    }
}