using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class ValidateFields(IConfiguration configuration, IResponseHandler responseHandler) : IValidateFields
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly string[] ValidFieldTypes = Enum.GetNames(typeof(FieldType));
        private readonly ResponseHandler _responseHandler = (ResponseHandler)responseHandler;

        public IActionResult ValidateElecate(GenerateFormRequestDTO request)
        {
            IActionResult initialValidationResult = ValidateInitialRequest(request);
            if (initialValidationResult is not OkResult)
            {
                return initialValidationResult;
            }

            List<FieldErrorDTO> errors = [];
            List<string> relationFieldsList = ["Select", "Radio", "Checkbox"];

            PropertyInfo[] fieldProperties = typeof(FieldGenerateFormRequestDTO).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(FieldGenerateFormRequestDTO)) || t == typeof(FieldGenerateFormRequestDTO))
                .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                .GroupBy(p => p.Name.ToLower())
                .Select(g => g.First())
                .ToArray();

            List<string>? omittedFieldElements = _configuration.GetSection("OmittedFieldElements").Get<List<string>>() ?? [];
            List<string>? requiredFieldElements = _configuration.GetSection("RequiredFieldElements").Get<List<string>>() ?? [];

            for (int i = 0; i < request.Fields!.Count; i++)
            {
                List<string> fieldErrors = [];
                FieldGenerateFormRequestDTO? field = (FieldGenerateFormRequestDTO?)request.Fields[i];

                if (field != null || request.OriginalJsonFields == null || request.OriginalJsonFields.Count <= i)
                {
                    if (field != null)
                    {
                        PropertyInfo[] properties = field.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                        for (int j = 0; j < properties.Length; j++)
                        {
                            PropertyInfo property = properties[j];
                            object? value = property.GetValue(field);

                            if (value == null &&
                                (field.Type != null &&
                                    relationFieldsList.Contains(field.Type) ?
                                    requiredFieldElements.Contains(property.Name) :
                                    !omittedFieldElements.Contains(property.Name)))
                            {
                                fieldErrors.Add($"The field '{property.Name}' is required");
                            }
                            else if (value is string stringValue &&
                                string.IsNullOrEmpty(stringValue) &&
                                (field.Type != null &&
                                    relationFieldsList.Contains(field.Type) ?
                                    requiredFieldElements.Contains(property.Name) :
                                    !omittedFieldElements.Contains(property.Name)))
                            {
                                fieldErrors.Add($"The field '{property.Name}' cannot be empty");
                            }
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo fieldProperty in fieldProperties)
                    {
                        string normalizedPropertyName = fieldProperty.Name.ToLower();

                        Dictionary<string, JsonElement> dictionaryOriginalJsonFields = request.OriginalJsonFields[i];
                        if (!dictionaryOriginalJsonFields.ContainsKey(normalizedPropertyName) && !omittedFieldElements.Contains(fieldProperty.Name))
                        {
                            fieldErrors.Add($"The field '{fieldProperty.Name}' is required");
                        }
                        else if (dictionaryOriginalJsonFields.TryGetValue(normalizedPropertyName, out JsonElement value))
                        {
                            if (value.ValueKind == JsonValueKind.Null ||
                                value.ValueKind == JsonValueKind.String &&
                                string.IsNullOrEmpty(value.GetString()) &&
                                !omittedFieldElements.Contains(fieldProperty.Name))
                            {
                                fieldErrors.Add($"The field '{fieldProperty.Name}' cannot be empty");
                            }
                            if (fieldProperty.Name.Equals("Type") && !ValidFieldTypes.Contains(value.GetString()))
                            {
                                fieldErrors.Add($"The value '{value.GetString()}' for field '{fieldProperty.Name}' does not exist");
                            }
                        }
                    }
                }

                if (fieldErrors.Count > 0)
                {
                    errors.Add(new FieldErrorDTO
                    {
                        Index = i,
                        FieldErrors = fieldErrors
                    });
                }
            }

            if (errors.Count > 0)
            {
                return _responseHandler.HandleError("Validation errors occurred",
                    HttpStatusCode.BadRequest,
                    new Exception(JsonSerializer.Serialize(errors)),
                    true,
                    errors);
            }

            return _responseHandler.HandleSuccess("Validation successful");
        }

        public IActionResult ValidateClientNameHoneypotFieldsExist(SaveFormDataRequestDTO request)
        {
            bool clientNameExists = false;
            bool honeypotFieldExists = false;
            List<string> errors = [];

            foreach (FieldSaveFormRequestDTO field in request.Fields!)
            {
                if (clientNameExists && honeypotFieldExists)
                {
                    return new OkResult();
                }

                if (!clientNameExists && field.Name!.Equals("ClientName", StringComparison.OrdinalIgnoreCase))
                {
                    clientNameExists = true;
                }

                if (!honeypotFieldExists && field.Name!.Equals("Honeypot", StringComparison.OrdinalIgnoreCase))
                {
                    honeypotFieldExists = true;
                    if (!string.IsNullOrEmpty(field.Value))
                    {
                        return _responseHandler.HandleError("Bot detected.", HttpStatusCode.BadRequest);
                    }
                }
            }

            if (!clientNameExists)
            {
                errors.Add("The field 'ClientName' is required");
            }

            if (!honeypotFieldExists)
            {
                errors.Add("The 'Honeypot' Validator is required");
            }

            if (errors.Count > 0)
            {
                var errorResponse = new GeneralErrorsResponseDTO
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Errors = errors
                };

                return new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            return new OkResult();
        }

        public IActionResult ValidateFieldLength(FieldSaveFormRequestDTO field, FormFieldsModel matchingField)
        {
            if (matchingField.Type == "Text" && matchingField.Size > 0 && field.Value?.Length > matchingField.Size)
            {
                return _responseHandler.HandleError($"The field '{field.Name}' exceeds the maximum length of {matchingField.Size}.", HttpStatusCode.BadRequest);
            }
            return new OkResult();
        }

        public IActionResult ValidateProspectData(Dictionary<string, object> prospectData)
        {
            if (prospectData.Count == 0)
            {
                return _responseHandler.HandleError("No valid fields found to insert.", HttpStatusCode.BadRequest);
            }
            return new OkResult();
        }

        public IActionResult ValidateProspect(ProspectModel prospect)
        {
            if (prospect == null)
            {
                return _responseHandler.HandleError("Prospect cannot be null.", HttpStatusCode.BadRequest);
            }

            return new OkResult();
        }

        public IActionResult ValidateUnmappedAndDuplicatedFields(Dictionary<string, int> fieldOccurrences,
                                                                 List<string> unmappedFields)
        {
            List<string> duplicatedFields = [.. fieldOccurrences
                .Where(kvp => kvp.Value > 1)
                .Select(kvp => kvp.Key)];

            if (unmappedFields.Count <= 0 && duplicatedFields.Count <= 0)
            {
                return new OkResult();
            }

            List<string> errorMessages = [];

            if (unmappedFields.Count > 0)
            {
                errorMessages.Add($"The following fields do not have a mapping in the database: {string.Join(", ", unmappedFields)}.");
            }

            if (duplicatedFields.Count > 0)
            {
                errorMessages.Add($"The following fields were sent multiple times: {string.Join(", ", duplicatedFields)}.");
            }

            return _responseHandler.HandleError(string.Join(" ", errorMessages), HttpStatusCode.BadRequest);
        }
        public IActionResult ValidateInitialRequest(SaveFormDataRequestDTO request)
        {
            if (request == null || request.Fields == null || request.Fields.Count == 0)
            {
                return _responseHandler.HandleError("You must provide at least one field", HttpStatusCode.BadRequest);
            }

            return new OkResult();
        }

        public void AddErrorIfNotOk(IActionResult validationResult, List<FieldErrorDTO> errors)
        {
            if (validationResult is BadRequestObjectResult badRequestResult)
            {
                if (badRequestResult.Value is StringErrorMessageResponseDTO stringError)
                {
                    errors.Add(new FieldErrorDTO
                    {
                        Index = -1,
                        FieldErrors = [stringError.Message]
                    });
                }
                else if (badRequestResult.Value is List<string> generalErrors)
                {
                    errors.Add(new FieldErrorDTO
                    {
                        Index = -1,
                        FieldErrors = generalErrors
                    });
                }
                else
                {
                    // Caso genérico si el formato no es el esperado
                    errors.Add(new FieldErrorDTO
                    {
                        Index = -1,
                        FieldErrors = [badRequestResult.Value?.ToString() ?? "Unknown error"]
                    });
                }
            }
        }

        public IActionResult ValidateInitialRequest(GenerateFormRequestDTO request)
        {
            if (request == null || request.Fields == null || request.Fields.Count == 0)
            {
                return _responseHandler.HandleError("You must provide at least one field", HttpStatusCode.BadRequest);
            }

            return new OkResult();
        }
    }
}