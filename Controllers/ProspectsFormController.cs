using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;

namespace ApiElecateProspectsForm.Controllers
{

    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController : ControllerBase
    {
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateHtmlForm([FromBody] GenerateFormRequestDTO request, [FromServices] FieldGeneratorFactory generatorFactory)
        {         
            IActionResult validationResult = ValidateFields.Validate(request);

            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            StringBuilder htmlBuilder = new();
            htmlBuilder.Append("<form>\n");

            foreach (var field in request.Fields)
            {
                if (field != null)
                {
                    if (!Enum.TryParse(field.Type, out FieldType fieldType))
                    {
                        fieldType = FieldType.Text;
                    }

                    var generator = generatorFactory.GetGenerator(fieldType);
                    string fieldHtml = await generator.GenerateComponent(field);
                    htmlBuilder.Append(fieldHtml);
                }       
            }

            htmlBuilder.Append("</form>");

            return Ok(htmlBuilder.ToString());
        }
    }
}
