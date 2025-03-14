﻿
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces.Repositories;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("elecate/prospects")]
    public class ProspectsFormController(
        IFormFieldsRepository formFieldsRepository,
        IResponseHandler responseHandler,
        IDbContextFactory dbContextFactory,
        IMaskFormatter maskFormatter,
        IValidateFields validateFields,
        IProspectMapper prospectMapper) : ControllerBase
    {
        private readonly IFormFieldsRepository _formFieldsRepository = formFieldsRepository;
        private readonly IResponseHandler _responseHandler = responseHandler;
        private readonly IDbContextFactory _dbContextFactory = dbContextFactory;
        private readonly IMaskFormatter _maskFormatter = maskFormatter;
        private readonly IValidateFields _validateFields = validateFields;
        private readonly IProspectMapper _prospectMapper = prospectMapper;

        [HttpPost("generate/{id}")]
        public async Task<IActionResult> GenerateHtmlForm(
            [FromBody] GenerateFormRequestDTO request,
            [FromServices] FieldGeneratorFactory generatorFactory,
            [FromServices] DbContextFactory dbContextFactory,
            int id)
        {
            // Validate the fields in the request
            IActionResult validationResult = _validateFields.ValidateElecate(request);

            if (validationResult is BadRequestObjectResult)
            {
                return validationResult;
            }

            // Initialize the HTML form builder
            StringBuilder htmlBuilder = new();
            htmlBuilder.Append("<form>\n");

            // Add hidden field for honeypot validation
            htmlBuilder.Append("<input type='hidden' id='honeypot' name='honeypot' value=''>\n");

            List<FormFieldsModel> fieldsToInsert = new();

            // Generate HTML for each field in the request
            foreach (FieldGenerateFormRequestDTO? field in request.Fields)
            {
                if (field != null)
                {
                    if (!Enum.TryParse(field.Type, true, out FieldType fieldType))
                    {
                        fieldType = FieldType.Text;
                    }

                    // Get the appropriate field generator and generate the field HTML
                    Interfaces.FormFieldsGenerators.IFormFieldGenerator generator = generatorFactory.GetGenerator(fieldType);
                    string fieldHtml = await generator.GenerateComponent(field);
                    htmlBuilder.Append(fieldHtml);

                    // Create the entity for saving fields in the database
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

            // Save fields in the database
            if (fieldsToInsert.Count != 0)
            {
                try
                {
                    await _formFieldsRepository.SyncFormFieldsAsync(id, fieldsToInsert);
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
            IActionResult initialValidationResult = _validateFields.ValidateInitialRequest(request);

            if (initialValidationResult is not OkResult)
            {
                return initialValidationResult;
            }

            IActionResult validateClientNameHoneypotFieldsExist = _validateFields.ValidateClientNameHoneypotFieldsExist(request);

            if (validateClientNameHoneypotFieldsExist is not OkResult)
            {
                return validateClientNameHoneypotFieldsExist;
            }

            try
            {
                List<FormFieldsModel> formFields = await _formFieldsRepository.GetFormFieldsAsync(id);

                await using ProspectDbContext dbContext = _dbContextFactory.CreateProspectDbContext(id.ToString());
                ProspectResultDTO prospectResult = _prospectMapper.MapRequestToProspect(request, formFields, _maskFormatter);

                if (!prospectResult.Success && prospectResult.Errors?.Count > 0)
                {
                    return _responseHandler.HandleError(
                        "An error ocurred while mapping fields",
                        HttpStatusCode.BadRequest,
                        null,
                        true,
                        prospectResult.Errors
                    );
                }

                if (prospectResult.Success && prospectResult.Prospect != null)
                {
                    dbContext.Prospect.Add(prospectResult.Prospect);

                    await dbContext.SaveChangesAsync();

                    return _responseHandler.HandleSuccess("Data saved successfully.");
                } else
                {
                    return _responseHandler.HandleError(
                        "An error ocurred while mapping fields",
                        HttpStatusCode.InternalServerError,
                        null,
                        false,
                        null
                    );
                }

            }
            catch (ArgumentException ex)
            {
                return _responseHandler.HandleError(ex.Message, HttpStatusCode.BadRequest);
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
