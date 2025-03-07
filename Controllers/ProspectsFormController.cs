using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using ApiElecateProspectsForm.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Controllers
{

    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController(IFormFieldsRepository formFieldsRepository) : ControllerBase
    {
        private IFormFieldsRepository _formFieldsRepository = formFieldsRepository;

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

        [HttpPost("saveData/{id}")]
        public async Task<IActionResult> SaveData([FromBody] SaveFormDataRequestDTO request, int id)
        {
            IQueryable<FormFieldsModel> formFields = _formFieldsRepository.GetFieldsByFormId(id);
            List<FormFieldsModel> formFieldsList = await formFields.ToListAsync();

            IActionResult validationResult = ValidateFormFields.ValidateFormFilds(formFields);

            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            return Ok(formFieldsList);
        }
    }
}
