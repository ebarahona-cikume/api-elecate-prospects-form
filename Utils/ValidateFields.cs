using ApiElecateProspectsForm.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Reflection;

namespace ApiElecateProspectsForm.Utils
{
    public static class ValidateFields
    {
        private static readonly IConfiguration _configuration;

        static ValidateFields()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();
        }

        public static IActionResult Validate(GenerateFormRequestDTO request)
        {
            if (request == null || request.Fields == null || request.Fields.Count == 0)
            {
                StringErrorMessageResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Message = "You must provide at least one field"
                };

                return new BadRequestObjectResult(errorResponse);
            }

            List<FieldErrorDTO> errors = new();

            PropertyInfo[] fieldProperties = typeof(FieldGenerateFormRequestDTO).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(FieldGenerateFormRequestDTO)) || t == typeof(FieldGenerateFormRequestDTO))
                .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                .GroupBy(p => p.Name.ToLower())
                .Select(g => g.First())
                .ToArray();

            List<string>? omittedFieldElements = _configuration.GetSection("OmittedFieldElements").Get<List<string>>() ?? new();
            List<string>? requiredFieldElements = _configuration.GetSection("RequiredFieldElements").Get<List<string>>() ?? new();

            for (int i = 0; i < request.Fields.Count; i++)
            {
                List<string> fieldErrors = new();
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

                        if (value == null && (field.Type != null &&
                            field.Type.Equals("Select") ? requiredFieldElements.Contains(property.Name) : !omittedFieldElements.Contains(property.Name)))
                        {
                            fieldErrors.Add($"The field '{property.Name}' is required");
                        }
                        else if (value is string stringValue && string.IsNullOrEmpty(stringValue) &&
                            (field.Type != null &&
                            field.Type.Equals("Select") ? requiredFieldElements.Contains(property.Name) : !omittedFieldElements.Contains(property.Name)))
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
                ArrayErrorMessageResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            }

            return new OkResult();
        }
    }
}