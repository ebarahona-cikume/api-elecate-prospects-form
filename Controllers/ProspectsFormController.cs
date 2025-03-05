using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json.Serialization;
using ApiElecateProspectsForm.Utils;

namespace ApiElecateProspectsForm.Controllers
{
    public class FormRequestDTO
    {
        [JsonPropertyName("fields")]
        public List<FormFieldRequestDTO>? Fields { get; set; }
    }


    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateHtmlForm([FromBody] FormRequestDTO request)
        {
            if (request == null || request.Fields == null || request.Fields.Count == 0)
            {
                return BadRequest("Debe proporcionar al menos un campo.");
            }

            //IActionResult validationResult = ValidateFields(request);

            //if (validationResult is BadRequestObjectResult)
            //{
            //    return validationResult;
            //}

            // validar que el campo "field.Type" sea igual a los valores de FieldType (Eddy)

            StringBuilder htmlBuilder = new();
            htmlBuilder.Append("<form>\n");

            foreach (var field in request.Fields)
            {
                FieldType fieldType = field.Type switch
                {
                    "text" => FieldType.Text,
                    "number" => FieldType.Number,
                    "password" => FieldType.Password,
                    "email" => FieldType.Email,
                    "select" => FieldType.Select,
                    "radio" => FieldType.Radio,
                    "checkbox" => FieldType.Checkbox,
                    _ => FieldType.Text
                };

                htmlBuilder.Append($"<label for=\"{field.Name}\">{field.Name}:</label>\n");
                   
                if (field is TextFieldRequestDTO textField)
                {
                    string maxLength = textField.Size > 0 ? $"maxlength=\"{textField.Size}\"" : "";
                    string mask = !string.IsNullOrEmpty(textField.Mask) ? $"mask=\"{textField.Mask}\"" : "";

                    htmlBuilder.Append($"<input type=\"{textField.Type}\" id=\"{textField.Name}\" name=\"{textField.Name}\" {maxLength} {mask} />\n");
                }
                else if (field is SelectFieldRequestDTO selectField)
                {
                    if (fieldType == FieldType.Select)
                    {
                        // implementar logica del Select (Salvador)
                    }
                    else if (fieldType == FieldType.Radio)
                    {
                        // implementar logica de  RadioButtons (Eddy)
                    }
                    else if (fieldType == FieldType.Checkbox)
                    {
                        // implementar logica del Checkbox (Salvador)
                    }
                }
            }

            htmlBuilder.Append("</form>");

            return Ok(htmlBuilder.ToString());
        }
    }
}
