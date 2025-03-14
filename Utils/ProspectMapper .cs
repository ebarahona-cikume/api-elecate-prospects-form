using ApiElecateProspectsForm.Controllers;
using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using System.Net;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class ProspectMapper(
        IValidateFields validateFields,
        IResponseHandler responseHandler) : IProspectMapper
    {
        private readonly IValidateFields _validateFields = validateFields;
        private readonly IResponseHandler _responseHandler = responseHandler;

        public object MapRequestToProspect(
            SaveFormDataRequestDTO request,
            List<FormFieldsModel> formFields,
            IMaskFormatter maskFormatter)
        {
            ProspectModel prospectModel = new();
            ProspectModel prospect = prospectModel;
            Dictionary<string, object> prospectData = [];

            List<FieldErrorDTO> errors = [];
            List<FieldErrorDTO> unmappedFields = []; // Fields not mapped in the database
            List<FieldErrorDTO> duplicatedFields = []; // Fields duplicated in the request

            // Dictionary to count occurrences and store indices of each field
            Dictionary<string, List<int>> fieldOccurrences = new(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < request.Fields!.Count; i++)
            {

                FieldSaveFormRequestDTO field = request.Fields[i];

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
                        prospectData[field.Name] = fieldValue;

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
                return _responseHandler.HandleError("Validation errors occurred",
                    HttpStatusCode.BadRequest,
                    new Exception(JsonSerializer.Serialize(errors)),
                    true,
                    errors);
            }

            return _responseHandler.HandleSuccess("Validation successful");
        }
    }
}