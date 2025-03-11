using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using ApiElecateProspectsForm.Interfaces;
using System.Net;
using ApiElecateProspectsForm.Context;

namespace ApiElecateProspectsForm.Controllers
{

    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController(IFormFieldsRepository formFieldsRepository, DbContextFactory dbContextFactory) : ControllerBase
    {
        private readonly DbContextFactory _dbContextFactory = dbContextFactory;

        [HttpPost("generate/{id}")]
        public async Task<IActionResult> GenerateHtmlForm([FromBody] GenerateFormRequestDTO request, [FromServices] FieldGeneratorFactory generatorFactory, [FromServices] DbContextFactory dbContextFactory, int id)
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

                    // Create the entity for saving fields in DB
                    var newField = new FormFieldsModel
                    {
                        IdForm = id,
                        Type = field.Type,
                        Name = field.Name,
                        Size = field is TextFieldRequestDTO textField ? textField.Size : null,
                        Mask = field is TextFieldRequestDTO textFieldWithMask ? textFieldWithMask.Mask : null,
                        Link = field.Link,
                        Relation = field is SelectFieldRequestDTO selectField ? selectField.Relation : null,
                        IsDeleted = false,
                    };

                    fieldsToInsert.Add(newField);
                }
            }

            htmlBuilder.Append("</form>");

            // Save fields in DB
            if (fieldsToInsert.Count != 0)
            {
                try
                {
                    await formFieldsRepository.SyncFormFieldsAsync(id, fieldsToInsert);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new StringErrorMessageResponseDTO
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Title = "Internal Server Error",
                        Message = ex.Message
                    });
                }
            }

            return Ok(htmlBuilder.ToString());
        }


        [HttpPost("saveData/{id}")]
        public async Task<IActionResult> SaveData([FromBody] SaveFormDataRequestDTO request, int id)
        {
            if (request?.Fields == null || request.Fields.Count == 0)
            {
                return BadRequest(new { Message = "The request must contain at least one field." });
            }

            try
            {
                // 1. Get the client database context dynamically
                await using var dbContext = _dbContextFactory.CreateProspectDbContext(id.ToString());

                // 2. Get the existing form fields
                //var formFields = await dbContext.FormFields_Tbl
                //    .Where(f => f.IdForm == id && !f.IsDeleted)
                //    .ToListAsync();

                //if (formFields.Count == 0)
                //{
                //    return BadRequest(new { Message = "No fields found for the given form ID." });
                //}

                // 3. Map the request fields to the corresponding columns
                var prospect = new Dictionary<string, object>();

                foreach (var field in request.Fields)
                {
                    //var matchingField = formFields.FirstOrDefault(f => f.Name == field.Name);

                    //if (matchingField != null)
                    //{
                    //    prospect[field.Name] = field.Value; // Assign the value dynamically
                    //}

                    prospect[field.Name ?? ""] = field.Value ?? ""; // Assign the value dynamically
                }

                if (prospect.Count == 0)
                {
                    return BadRequest(new { Message = "No valid fields found to insert." });
                }

                // 4. Insert the new record dynamically into the Prospects table
                var prospectEntity = new ProspectModel();

                foreach (var entry in prospect)
                {
                    var property = typeof(ProspectModel).GetProperty(entry.Key);

                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(prospectEntity, Convert.ChangeType(entry.Value, property.PropertyType));
                    }

                    //prospectEntity["ClientName"] = "Barceló";

                    prospectEntity.ClientName = "Barceló";
                }

                dbContext.Prospect.Add(prospectEntity);
                await dbContext.SaveChangesAsync();

                return Ok(new { Message = "Data saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while saving data.",
                    Error = ex.Message
                });
            }
        }
    }
}



