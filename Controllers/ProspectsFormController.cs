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

        public DbContextFactory DbContextFactory => _dbContextFactory;

        [HttpPost("generate/{id}")]
        public async Task<IActionResult> GenerateHtmlForm(
            [FromBody] GenerateFormRequestDTO request, 
            [FromServices] FieldGeneratorFactory generatorFactory, 
            [FromServices] DbContextFactory dbContextFactory, 
            int id)
        {
            IActionResult validationResult = ValidateFields.Validate(request);

            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            StringBuilder htmlBuilder = new();
            htmlBuilder.Append("<form>\n");

            //Add hidden field honeypot validation
            htmlBuilder.Append("<input type='hidden' id='honeypot' name='honeypot' value='' style='display:none;'>\n");

            List<FormFieldsModel> fieldsToInsert = [];

            foreach (FieldGenerateFormRequestDTO? field in request.Fields)
            {
                if (field != null)
                {
                    if (!Enum.TryParse(field.Type, out FieldType fieldType))
                    {
                        fieldType = FieldType.Text;
                    }

                    Interfaces.FormFieldsGenerators.IFormFieldGenerator generator = generatorFactory.GetGenerator(fieldType);
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
                return HandleError("The request must contain at least one field.", HttpStatusCode.BadRequest);
            }

            try
            {
                // 1. Get the client database context dynamically
                await using ProspectDbContext dbContext = _dbContextFactory.CreateProspectDbContext(id.ToString());

                // 2. Get the existing form fields
                //var formFields = await dbContext.FormFields_Tbl
                //    .Where(f => f.IdForm == id && !f.IsDeleted)
                //    .ToListAsync();

                //if (formFields.Count == 0)
                //{
                //    return BadRequest(new { Message = "No fields found for the given form ID." });
                //}

                // 3. Map the request fields to the corresponding columns
                Dictionary<string, object> prospect = new Dictionary<string, object>();
                bool honeypotFieldExists = false;

                foreach (FieldSaveFormRequestDTO field in request.Fields)
                {
                    // Honeypot validation
                    if (!string.IsNullOrEmpty(field.Name) && field.Name.Equals("Honeypot", StringComparison.OrdinalIgnoreCase))
                    {
                        honeypotFieldExists = true;
                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            return HandleError("Bot detected.", HttpStatusCode.BadRequest);
                        }
                    }
                    //var matchingField = formFields.FirstOrDefault(f => f.Name == field.Name);

                    //if (matchingField != null)
                    //{
                    //    prospect[field.Name] = field.Value; // Assign the value dynamically
                    //}

                    prospect[field.Name ?? ""] = field.Value ?? ""; // Assign the value dynamically
                }

                if (!honeypotFieldExists)
                {
                    return HandleError("Honeypot field is missing.", HttpStatusCode.BadRequest);
                }

                if (prospect.Count == 0)
                {
                    return HandleError("No valid fields found to insert.", HttpStatusCode.BadRequest);
                }

                // 4. Insert the new record dynamically into the Prospects table
                var prospectEntity = new ProspectModel();

                foreach (KeyValuePair<string, object> entry in prospect)
                {
                    System.Reflection.PropertyInfo? property = typeof(ProspectModel).GetProperty(entry.Key);

                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(prospectEntity, Convert.ChangeType(entry.Value, property.PropertyType));
                    }
                }

                dbContext.Prospect.Add(prospectEntity);
                await dbContext.SaveChangesAsync();

                return HandleSuccess("Data saved successfully.");
            }
            catch (Exception ex)
            {
                return HandleError("An error occurred while saving data.", HttpStatusCode.InternalServerError, ex);
            }
        }

        private IActionResult HandleSuccess(string message) => Ok(new StringSuccessMessageResponseDTO
        {
            Status = HttpStatusCode.OK,
            Title = "Success",
            Message = message
        });

        private IActionResult HandleError(string message, 
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError, 
            Exception? ex = null) => StatusCode((int)statusCode, 
                new StringErrorMessageResponseDTO
        {
            Status = statusCode,
            Title = statusCode == HttpStatusCode.BadRequest ? "Bad Request" : "Internal Server Error",
            Message = ex?.Message ?? message
        });
    }
}
