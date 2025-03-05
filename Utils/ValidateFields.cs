using ApiElecateProspectsForm.Controllers;
using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiElecateProspectsForm.Utils
{
    public class ValidateFields
    {
        //private static IActionResult ValidateFields(FormRequestDTO request)
        //{
        //    if (request.Fields == null)
        //    {
        //        return BadRequestResult("El campo 'fields' es obligatorio.");
        //    }

        //    foreach (var property in from FormFieldRequestDTO field in request.Fields
        //                             let properties = field.GetType().GetProperties()
        //                             from System.Reflection.PropertyInfo property in properties
        //                             let value = property.GetValue(field) as string
        //                             where property.PropertyType == typeof(string) && string.IsNullOrEmpty(value) && property.Name != "Mask"
        //                             select property)
        //    {
        //        return BadRequest($"El campo '{property.Name}' es obligatorio.");
        //    }

        //    return Ok();
        //}
    }
}
