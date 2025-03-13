using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using ApiElecateProspectsForm.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using System.Text.Json;

public class ValidateFields(IConfiguration configuration, IResponseHandler responseHandler) : IValidateFields
{
    private readonly IConfiguration _configuration = configuration;
    private readonly string[] ValidFieldTypes = Enum.GetNames(typeof(FieldType));
    private readonly ResponseHandler _responseHandler = (ResponseHandler)responseHandler;

    public IActionResult ValidateElecate(GenerateFormRequestDTO request)
    {
        if (request == null || request.Fields == null || request.Fields.Count == 0)
        {
            return _responseHandler.HandleError("You must provide at least one field", HttpStatusCode.BadRequest);
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

        for (int i = 0; i < request.Fields.Count; i++)
        {
            List<string> fieldErrors = [];
            FieldGenerateFormRequestDTO? field = request.Fields[i];

            if (field == null && request.OriginalJsonFields != null && request.OriginalJsonFields.Count > i)
            {
                Dictionary<string, JsonElement> dictionaryOriginalJsonFields = request.OriginalJsonFields[i];

                foreach (PropertyInfo fieldProperty in fieldProperties)
                {
                    string normalizedPropertyName = fieldProperty.Name.ToLower();

                    if (!dictionaryOriginalJsonFields.ContainsKey(normalizedPropertyName) && !omittedFieldElements.Contains(fieldProperty.Name))
                    {
                        fieldErrors.Add($"The field '{fieldProperty.Name}' is required");
                    }
                    else if (dictionaryOriginalJsonFields.TryGetValue(normalizedPropertyName, out JsonElement value))
                    {
                        if (value.ValueKind == JsonValueKind.Null ||
                            (value.ValueKind == JsonValueKind.String &&
                            string.IsNullOrEmpty(value.GetString()) &&
                            !omittedFieldElements.Contains(fieldProperty.Name)))
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
            else if (field != null)
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

    public IActionResult ValidateField(FieldSaveFormRequestDTO field, string fieldName)
    {
        if (!string.IsNullOrEmpty(field.Name) && field.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
        {
            if (fieldName.Equals("Honeypot", StringComparison.OrdinalIgnoreCase))
            {
                GlobalStateDTO.HoneypotFieldExists = true;
                if (!string.IsNullOrEmpty(field.Value))
                {
                    return _responseHandler.HandleError("Bot detected.", HttpStatusCode.BadRequest);
                }
            }
            else if (fieldName.Equals("ClientName", StringComparison.OrdinalIgnoreCase))
            {
                GlobalStateDTO.ClientNameExists = true;
            }
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

    public IActionResult ValidateHoneypotFieldExists()
    {
        if (!GlobalStateDTO.HoneypotFieldExists)
        {
            return _responseHandler.HandleError("Honeypot Validator is required.", HttpStatusCode.BadRequest);
        }
        return new OkResult();
    }

    public IActionResult ValidateClientNameFieldExists()
    {
        if (!GlobalStateDTO.ClientNameExists)
        {
            return _responseHandler.HandleError("ClientName is required.", HttpStatusCode.BadRequest);
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
}