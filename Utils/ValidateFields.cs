using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class ValidateFields(
        IConfiguration configuration, 
        IResponseHandler responseHandler,
        IOptions<FieldNamesConfigDTO> fieldNamesConfigDTO) : IValidateFields
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly FieldNamesConfigDTO _fieldNamesConfigDTO = fieldNamesConfigDTO.Value;
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
                        Index = i.ToString(),
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
            List<string> errors = [];

            FieldSaveFormRequestDTO? honeypotField = request.Fields!.FirstOrDefault(field => field.Name!.Equals(_fieldNamesConfigDTO.Honeypot, StringComparison.OrdinalIgnoreCase));
            bool honeypotFieldExists = honeypotField != null;
            bool honeypotFieldHasValue = honeypotFieldExists && !string.IsNullOrEmpty(honeypotField!.Value);

            //if (!ValidateClientNameExists(request.Fields!))
            //{
            //    errors.Add("The field 'ClientName' is required");
            //}

            if (!honeypotFieldExists)
            {
                errors.Add("The field 'Honeypot' is required");
            }
            else if (honeypotFieldHasValue)
            {
                errors.Add("Bot detected!");
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

        public void ValidateFieldLength(FieldSaveFormRequestDTO field, FormFieldsModel matchingField, int index, List<FieldErrorDTO> errors)
        {
            if (matchingField.Type == "Text")
            {
                string message = string.Empty;

                if (matchingField.Size > 0 && field.Value?.Length > matchingField.Size)
                {
                    message = $"The field '{field.Name}' exceeds the maximum length of {matchingField.Size}.";
                }

                if (!(field.Value?.Length > 0))
                {
                    message = $"The field '{field.Name}' cannot be empty";
                }

                if (message.Length > 0)
                {
                    errors.Add(new FieldErrorDTO
                    {
                        Index = index.ToString(),
                        FieldErrors =
                        [
                            message
                        ]
                    });
                }
            }
        }


        public bool ValidateClientNameExists(List<FieldGenerateFormRequestDTO> fields)
        {
            return fields.Any(field => field.Name!.Equals(_fieldNamesConfigDTO.ClientName, StringComparison.OrdinalIgnoreCase));
        }

        public bool ValidateClientNameExists(List<FieldSaveFormRequestDTO> fields)
        {
            return fields.Any(field => field.Name!.Equals(_fieldNamesConfigDTO.ClientName, StringComparison.OrdinalIgnoreCase));
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

        public IActionResult ValidateUnmappedAndDuplicatedFields(List<FieldErrorDTO> repeatedFields, List<FieldErrorDTO> unmappedFields)
        {
            List<FieldErrorDTO> errorList = [];

            if (unmappedFields.Count <= 0 && repeatedFields.Count <= 0)
            {
                return new OkResult();
            }

            // Agregar errores de campos no mapeados
            foreach (FieldErrorDTO field in unmappedFields)
            {
                errorList.Add(field);
            }

            // Agregar errores de campos duplicados
            foreach (FieldErrorDTO field in repeatedFields)
            {
                errorList.Add(field);
            }

            return _responseHandler.HandleError("", HttpStatusCode.BadRequest, null, true, errorList);
        }

        public IActionResult ValidateInitialRequest(SaveFormDataRequestDTO request)
        {
            if (request == null || request.Fields == null || request.Fields.Count == 0)
            {
                return _responseHandler.HandleError("You must provide at least one field", HttpStatusCode.BadRequest);
            }

            return new OkResult();
        }

        public void AddErrorIfNotOk(IActionResult validationResult, List<FieldErrorDTO> errors, string index = "N/A")
        {
            if (validationResult is BadRequestObjectResult badRequestResult)
            {
                switch (badRequestResult.Value)
                {
                    case StringErrorMessageResponseDTO stringError:
                        errors.Add(new FieldErrorDTO
                        {
                            Index = index, // Se recibe como parámetro para ser más flexible
                            FieldErrors = [stringError.Message]
                        });
                        break;

                    case List<string> generalErrors:
                        errors.Add(new FieldErrorDTO
                        {
                            Index = index,
                            FieldErrors = generalErrors
                        });
                        break;

                    default:
                        errors.Add(new FieldErrorDTO
                        {
                            Index = index,
                            FieldErrors = [badRequestResult.Value?.ToString() ?? "Unknown error"]
                        });
                        break;
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