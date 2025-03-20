using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Utils
{
    public class ProspectMapper(IValidateFields validateFields) : IProspectMapper
    {
        private readonly IValidateFields _validateFields = validateFields;

        public ProspectResultDTO MapRequestToProspect(
        SaveFormDataRequestDTO request,
        List<FormFieldsModel> formFields,
        IMaskFormatter maskFormatter)
        {
            ProspectModel prospectEntity = new();

            List<FieldErrorDTO> errors = [];
            List<FieldErrorDTO> unmappedFields = []; // Fields not mapped in the database
            List<FieldErrorDTO> duplicatedFields = []; // Fields duplicated in the request

            // Dictionary to count occurrences and store indices of each field
            Dictionary<string, List<int>> fieldOccurrences = new(StringComparer.OrdinalIgnoreCase);

            // Obtener todas las propiedades de ProspectModel para asignación dinámica
            System.Reflection.PropertyInfo[] properties = typeof(ProspectModel).GetProperties();

            for (int i = 0; i < request.Fields!.Count; i++)
            {
                FieldSaveFormRequestDTO field = request.Fields[i];

                // Skip the field if it's a honeypot
                if (field.Name?.Equals("Honeypot", StringComparison.OrdinalIgnoreCase) == true)
                {
                    continue;
                }

                FormFieldsModel? matchingField = formFields
                    .FirstOrDefault(f => f.Name != null && f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

                if (matchingField != null)
                {
                    // Validate the length of the field
                    _validateFields.ValidateFieldLength(field, matchingField, i, errors);

                    // Apply mask if it exists
                    string fieldValue = field.Value ?? "";

                    if (!string.IsNullOrEmpty(matchingField.Mask))
                    {
                        fieldValue = maskFormatter.ApplyMask(fieldValue, matchingField.Mask);
                    }

                    if (!string.IsNullOrEmpty(field.Name))
                    {
                        // Buscar la propiedad en ProspectModel que coincida con el nombre del campo
                        System.Reflection.PropertyInfo? property = properties
                            .FirstOrDefault(p => p.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

                        if (property != null && property.CanWrite)
                        {
                            try
                            {
                                property.SetValue(prospectEntity, Convert.ChangeType(fieldValue, property.PropertyType));
                            }
                            catch (Exception ex)
                            {
                                errors.Add(new FieldErrorDTO
                                {
                                    Index = i.ToString(),
                                    FieldErrors = [$"Error setting value for '{field.Name}': {ex.Message}"]
                                });
                            }
                        }

                        // Save the occurrence of the field with its index
                        if (!fieldOccurrences.TryGetValue(field.Name, out List<int>? value))
                        {
                            fieldOccurrences[field.Name] = [i];
                        }
                        else
                        {
                            value.Add(i);
                        }
                    }
                }
                else
                {
                    // Save unmapped field with its index
                    unmappedFields.Add(new FieldErrorDTO
                    {
                        Index = i.ToString(),
                        FieldErrors = [$"The field '{field.Name}' is not mapped in the database."]
                    });
                }
            }

            // Identify and register duplicated fields
            foreach (var entry in fieldOccurrences)
            {
                if (entry.Value.Count > 1) // If the field appeared more than once
                {
                    duplicatedFields.Add(new FieldErrorDTO
                    {
                        Index = string.Join(", ", entry.Value), // Convert indices to a comma-separated string
                        FieldErrors = [$"The field '{entry.Key}' has been sent multiple times."]
                    });
                }
            }

            foreach (FieldErrorDTO duplicatedField in duplicatedFields)
            {
                errors.Add(duplicatedField);
            }

            foreach (FieldErrorDTO unmappedField in unmappedFields)
            {
                errors.Add(unmappedField);
            }

            if (errors.Count > 0)
            {
                return new ProspectResultDTO
                {
                    Success = false,
                    Errors = errors
                };
            }

            return new ProspectResultDTO
            {
                Success = true,
                Prospect = prospectEntity
            };
        }
    }
}