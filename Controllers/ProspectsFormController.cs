using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using ApiElecateProspectsForm.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApiElecateProspectsForm.Controllers
{

    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController(IFormFieldsRepository formFieldsRepository) : ControllerBase
    {
        private readonly IFormFieldsRepository _formFieldsRepository = formFieldsRepository;

        [HttpPost("generate/{id}")]
        public async Task<IActionResult> GenerateHtmlForm([FromBody] GenerateFormRequestDTO request, [FromServices] FieldGeneratorFactory generatorFactory, int id)
        {         
            IActionResult validationResult = ValidateFields.Validate(request);

            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            StringBuilder htmlBuilder = new();
            htmlBuilder.Append("<form>\n");

            List<FormFieldsModel> fieldsToInsert = [];

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

                    // Create the entity for DB
                    var newField = new FormFieldsModel
                    {
                        IdForm = id, // Save the form id
                        Type = field.Type,
                        Name = field.Name,
                        Size = field is TextFieldRequestDTO textField ? textField.Size : null,
                        Mask = field is TextFieldRequestDTO textFieldWithMask ? textFieldWithMask.Mask : null,
                        Link = field.Link,
                        Relation = field is SelectFieldRequestDTO selectField ? selectField.Relation : null
                    };

                    fieldsToInsert.Add(newField);
                }       
            }

            htmlBuilder.Append("</form>");

            // Save all the fields in DB
            if (fieldsToInsert.Count != 0)
            {
                try
                {
                    await formFieldsRepository.ReplaceFieldsAsync(id, fieldsToInsert);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new StringErrorMessageResponseDTO { Status = HttpStatusCode.InternalServerError, Title = "Internal Server Error", Message = ex.Message });
                }
            }

            return Ok(htmlBuilder.ToString());
        }

        [HttpPost("saveData/{id}")]
        public async Task<IActionResult> SaveData([FromBody] SaveFormDataRequestDTO request, int id)
        {
            IQueryable<FormFieldsModel> formFields = _formFieldsRepository.GetFieldsByFormId(id);
            var formFieldsList = await formFields.ToListAsync();
            return Ok(formFieldsList);
        }
    }


}
