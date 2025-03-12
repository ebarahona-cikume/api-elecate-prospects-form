using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using System.Net;
using ApiElecateProspectsForm.Context;
using Microsoft.EntityFrameworkCore;
using ApiElecateProspectsForm.Interfaces.Repositories;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController(IFormFieldsRepository formFieldsRepository, DbContextFactory dbContextFactory) : ControllerBase
    {
        private readonly DbContextFactory _dbContextFactory = dbContextFactory;
        private readonly ResponseHandler _responseHandler = new();

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
            htmlBuilder.Append("<input type='hidden' id='honeypot' name='honeypot' value=''>\n");

            List<FormFieldsModel> fieldsToInsert = [];

            foreach (FieldGenerateFormRequestDTO? field in request.Fields)
            {
                if (field != null)
                {
                    if (!Enum.TryParse(field.Type, true, out FieldType fieldType))
                    {
                        fieldType = FieldType.Text;
                    }

                    Interfaces.FormFieldsGenerators.IFormFieldGenerator generator = generatorFactory.GetGenerator(fieldType);
                    string fieldHtml = await generator.GenerateComponent(field);
                    htmlBuilder.Append(fieldHtml);

                    // Create the entity for saving fields in DB
                    FormFieldsModel newField = new()
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
                    return _responseHandler.HandleError("Internal Server Error.", HttpStatusCode.BadRequest, ex);
                }
            }

            return Ok(htmlBuilder.ToString());
        }

        [HttpPost("saveData/{id}")]
        public async Task<IActionResult> SaveData([FromBody] SaveFormDataRequestDTO request, int id)
        {
            if (request?.Fields == null || request.Fields.Count == 0)
            {
                return _responseHandler.HandleError("The request must contain at least one field.", HttpStatusCode.BadRequest);
            }

            try
            {
                var formFields = await GetFormFields(id);
                if (formFields.Count == 0)
                {
                    return _responseHandler.HandleError("No fields found for the given form ID.", HttpStatusCode.BadRequest);
                }

                await using var dbContext = _dbContextFactory.CreateProspectDbContext(id.ToString());

                var (prospect, honeypotFieldExists, clientNameExists, unmappedFields, duplicatedFields) = MapRequestFields(request, formFields);

                if (!honeypotFieldExists)
                {
                    return _responseHandler.HandleError("Honeypot field is missing.", HttpStatusCode.BadRequest);
                }

                if (!clientNameExists)
                {
                    return _responseHandler.HandleError("ClientName field is missing.", HttpStatusCode.BadRequest);
                }

                if (unmappedFields.Count > 0 || duplicatedFields.Count > 0)
                {
                    return HandleFieldValidationErrors(unmappedFields, duplicatedFields);
                }

                if (prospect.Count == 0)
                {
                    return _responseHandler.HandleError("No valid fields found to insert.", HttpStatusCode.BadRequest);
                }

                var prospectEntity = MapProspectEntity(prospect);
                dbContext.Prospect.Add(prospectEntity);
                await dbContext.SaveChangesAsync();

                return _responseHandler.HandleSuccess("Data saved successfully.");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private async Task<List<FormFieldsModel>> GetFormFields(int formId)
        {
            await using var elecateDbContext = _dbContextFactory.CreateElecateDbContext();
            return await elecateDbContext.FormFields_Tbl
                .Where(f => f.IdForm == formId && !f.IsDeleted)
                .ToListAsync();
        }

        private static (Dictionary<string, object> Prospect, bool HoneypotFieldExists, bool ClientNameExists, List<string> UnmappedFields, List<string> DuplicatedFields)
            MapRequestFields(SaveFormDataRequestDTO request, List<FormFieldsModel> formFields)
        {
            var prospect = new Dictionary<string, object>();
            var honeypotFieldExists = false;
            var clientNameExists = false;
            var unmappedFields = new List<string>();
            var duplicatedFields = new List<string>();

            var fieldOccurrences = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in request.Fields)
            {
                if (field.Name != null)
                {
                    if (field.Name.Equals("Honeypot", StringComparison.OrdinalIgnoreCase))
                    {
                        honeypotFieldExists = true;
                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            throw new ArgumentException("Bot detected.");
                        }
                    }

                    if (field.Name.Equals("ClientName", StringComparison.OrdinalIgnoreCase))
                    {
                        clientNameExists = true;
                    }

                    fieldOccurrences[field.Name] = fieldOccurrences.GetValueOrDefault(field.Name, 0) + 1;

                    var matchingField = formFields.FirstOrDefault(f => f.Name != null && f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));
                    if (matchingField != null)
                    {
                        prospect[field.Name] = field.Value ?? "";
                    }
                    else
                    {
                        unmappedFields.Add(field.Name);
                    }
                }
            }

            duplicatedFields = fieldOccurrences
                .Where(kvp => kvp.Value > 1)
                .Select(kvp => kvp.Key)
                .ToList();

            return (prospect, honeypotFieldExists, clientNameExists, unmappedFields, duplicatedFields);
        }

        private IActionResult HandleFieldValidationErrors(List<string> unmappedFields, List<string> duplicatedFields)
        {
            var errorMessages = new List<string>();

            if (unmappedFields.Count > 0)
            {
                errorMessages.Add($"The following fields do not have a mapping in the database: '{string.Join(", ", unmappedFields)}'.");
            }

            if (duplicatedFields.Count > 0)
            {
                errorMessages.Add($"The following fields were sent multiple times: '{string.Join(", ", duplicatedFields)}'.");
            }

            return _responseHandler.HandleError("Validation errors occurred", HttpStatusCode.BadRequest, null, true, null, errorMessages);
        }

        private static ProspectModel MapProspectEntity(Dictionary<string, object> prospect)
        {
            var prospectEntity = new ProspectModel();

            foreach (var entry in prospect)
            {
                var property = typeof(ProspectModel).GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(entry.Key, StringComparison.OrdinalIgnoreCase));

                if (property != null && property.CanWrite)
                {
                    property.SetValue(prospectEntity, Convert.ChangeType(entry.Value, property.PropertyType));
                }
            }

            return prospectEntity;
        }

        private IActionResult HandleException(Exception ex)
        {
            var errorMessage = "An error occurred while saving data.";
            if (ex.InnerException != null)
            {
                errorMessage += $" Inner exception: {ex.InnerException.Message}";
            }
            return _responseHandler.HandleError(errorMessage, HttpStatusCode.InternalServerError, ex);
        }
    }
}