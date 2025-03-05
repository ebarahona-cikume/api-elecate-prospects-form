using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Controllers
{

    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController : ControllerBase
    {
        //private readonly IGenerateFormComponent _generateFormComponent;

        //public ProspectsFormController(IGenerateFormComponent generateFormComponent)
        //{
        //    _generateFormComponent = generateFormComponent;
        //}

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProspectsFormController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateHtmlForm([FromBody] GenerateFormRequestDTO request)
        {         
            IActionResult validationResult = ValidateFields.Validate(request);

                if (validationResult is BadRequestObjectResult)
                {
                    return validationResult;
                }


                StringBuilder htmlBuilder = new();
                htmlBuilder.Append("<form>\n");

                var fields = request.Fields;

                foreach (var (field, fieldType) in from FormFieldRequestDTO field in fields
                                                   let fieldType = field.Type switch
                                                   {
                                                       "Text" => FieldType.Text,
                                                       "Number" => FieldType.Number,
                                                       "Password" => FieldType.Password,
                                                       "Email" => FieldType.Email,
                                                       "Select" => FieldType.Select,
                                                       "Radio" => FieldType.Radio,
                                                       "Checkbox" => FieldType.Checkbox,
                                                       _ => FieldType.Text
                                                   }
                                                   select (field, fieldType))
                {
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
                            //_generateFormComponent.GenerateComponent(htmlBuilder, field);

                            try
                            {
                                string? maritalStatusUrl = _configuration["ApiUrls:MaritalStatusUrl"];

                                HttpResponseMessage response = await _httpClient.GetAsync(maritalStatusUrl);
                                response.EnsureSuccessStatusCode();

                                IEnumerable<MaritalStatusModel> maritalStatuses = await response.Content.ReadFromJsonAsync<IEnumerable<MaritalStatusModel>>() ??
                                    Enumerable.Empty<MaritalStatusModel>();

                                htmlBuilder.Append($"<select id=\"{field.Name}\" name=\"{field.Name}\">\n");
                                foreach (MaritalStatusModel status in maritalStatuses)
                                {
                                    htmlBuilder.Append($"<option value=\"{status.Id}\">{status.MaritalStatus?.Trim()}</option>\n");
                                }
                                htmlBuilder.Append("</select>\n");
                            }
                            catch (HttpRequestException httpEx)
                            {
                                return StatusCode(500, $"Error fetching data: {httpEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
                            }
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
