using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ApiElecateProspectsForm.Utils;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using ApiElecateProspectsForm.Interfaces;
using System.Net;
using ApiElecateProspectsForm.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController(IFormFieldsRepository formFieldsRepository, DbContextFactory dbContextFactory) : ControllerBase
    {
        private readonly DbContextFactory _dbContextFactory = dbContextFactory;
        private readonly ResponseHandler _responseHandler = new();

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
                // 1. Get the form fields from Elecate_DB
                List<FormFieldsModel> formFields;
                await using (ElecateDbContext elecateDbContext = _dbContextFactory.CreateElecateDbContext())
                {
                    formFields = await elecateDbContext.FormFields_Tbl
                        .Where(f => f.IdForm == id && !f.IsDeleted)
                        .ToListAsync();
                }

                if (formFields.Count == 0)
                {
                    return _responseHandler.HandleError("No fields found for the given form ID.", HttpStatusCode.BadRequest);
                }

                // 2. Get the client database context dynamically
                await using ProspectDbContext dbContext = _dbContextFactory.CreateProspectDbContext(id.ToString());

                // 3. Map the request fields to the corresponding columns
                Dictionary<string, object> prospect = [];
                bool honeypotFieldExists = false;
                bool clientNameExists = false;

                foreach (FieldSaveFormRequestDTO field in request.Fields)
                {
                    // Honeypot validation
                    if (!string.IsNullOrEmpty(field.Name) && field.Name.Equals("Honeypot", StringComparison.OrdinalIgnoreCase))
                    {
                        honeypotFieldExists = true;
                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            return _responseHandler.HandleError("Bot detected.", HttpStatusCode.BadRequest);
                        }
                    }

                    //ClientName validation
                    if (!string.IsNullOrEmpty(field.Name) && field.Name.Equals("ClientName", StringComparison.OrdinalIgnoreCase))
                    {
                        clientNameExists = true;
                    }

                    if (!string.IsNullOrEmpty(field.Name))
                    {
                        FormFieldsModel? matchingField = formFields
                            .FirstOrDefault(f => f.Name != null && f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

                        if (matchingField != null)
                        {
                            prospect[field.Name] = field.Value ?? ""; // Assign the value dynamically
                        }
                    }
                }

                //  Honeypot field exists.
                if (!honeypotFieldExists)
                {
                    return _responseHandler.HandleError("Honeypot field is missing.", HttpStatusCode.BadRequest);
                }

                //  ClientName field is missing.
                if (!clientNameExists)
                {
                    return _responseHandler.HandleError("ClientName field is missing.", HttpStatusCode.BadRequest);
                }

                // All Prospects fiels are missing.
                if (prospect.Count == 0)
                {
                    return _responseHandler.HandleError("No valid fields found to insert.", HttpStatusCode.BadRequest);
                }

                // 4. Insert the new record dynamically into the Prospects table
                ProspectModel prospectEntity = new();

                foreach (KeyValuePair<string, object> entry in prospect)
                {
                    // Obtener todas las propiedades de ProspectModel
                    System.Reflection.PropertyInfo[] properties = typeof(ProspectModel).GetProperties();

                    // Buscar la propiedad que coincida con el nombre, ignorando mayúsculas y minúsculas
                    System.Reflection.PropertyInfo? property = properties.FirstOrDefault(p => p.Name.Equals(entry.Key, StringComparison.OrdinalIgnoreCase));

                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(prospectEntity, Convert.ChangeType(entry.Value, property.PropertyType));
                    }
                }

                dbContext.Prospect.Add(prospectEntity);
                await dbContext.SaveChangesAsync();

                return _responseHandler.HandleSuccess("Data saved successfully.");
            }
            catch (Exception ex)
            {
                string errorMessage = "An error occurred while saving data.";

                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner exception: {ex.InnerException.Message}";
                }

                return _responseHandler.HandleError(errorMessage, HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
