using ApiElecateProspectsForm.Controllers;
using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class ProspectMapper(IValidateFields validateFields, IResponseHandler responseHandler) : IProspectMapper
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
            Dictionary<string, object> prospectData = new();

            // Reset the state of HoneypotFieldExists
            GlobalStateDTO.HoneypotFieldExists = false;
            GlobalStateDTO.ClientNameExists = false;

            IActionResult initialValidationResult = _validateFields.ValidateInitialRequest(request);

            if (initialValidationResult is not OkResult)
            {
                return initialValidationResult;
            }

            List<FieldErrorDTO> errors = new();
            List<string> unmappedFields = new(); // List of fields not mapped in the database
            List<string> duplicatedFields = new(); // List of duplicate fields in the request

            // Dictionary to count the frequency of each field sent in the JSON
            Dictionary<string, int> fieldOccurrences = new(StringComparer.OrdinalIgnoreCase);

            foreach (FieldSaveFormRequestDTO field in request.Fields!)
            {
                if (!GlobalStateDTO.HoneypotFieldExists || !GlobalStateDTO.ClientNameExists)
                {
                    // Honeypot validation
                    _validateFields.AddErrorIfNotOk(_validateFields.ValidateField(field, "Honeypot"), errors);

                    // ClientName validation
                    _validateFields.AddErrorIfNotOk(_validateFields.ValidateField(field, "ClientName"), errors);
                }

                FormFieldsModel? matchingField = formFields.FirstOrDefault(f => f.Name != null && f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

                if (matchingField != null)
                {
                    _validateFields.AddErrorIfNotOk(_validateFields.ValidateFieldLength(field, matchingField), errors);

                    // Apply the mask if it exists
                    string fieldValue = field.Value ?? "";

                    if (!string.IsNullOrEmpty(matchingField.Mask))
                    {
                        fieldValue = maskFormatter.ApplyMask(fieldValue, matchingField.Mask);
                    }

                    if (!string.IsNullOrEmpty(field.Name))
                    {
                        prospectData[field.Name] = fieldValue;  // Assign the masked value

                        // Count the occurrences of each field
                        fieldOccurrences[field.Name] = fieldOccurrences.TryGetValue(field.Name, out int value) ? ++value : 1;

                        if (matchingField != null)
                        {
                            prospectData[field.Name] = field.Value ?? ""; // Dynamically assign the value
                        }
                        else
                        {
                            unmappedFields.Add(field.Name); // Add the unmapped field to the list
                        }
                    }
                }
            }

            // Check if honeypot field exists
            _validateFields.AddErrorIfNotOk(_validateFields.ValidateHoneypotFieldExists(), errors);

            // Check if ClientName field exists
            _validateFields.AddErrorIfNotOk(_validateFields.ValidateClientNameFieldExists(), errors);

            // Validate unmapped and duplicated fields
            _validateFields.AddErrorIfNotOk(_validateFields.ValidateUnmappedAndDuplicatedFields(fieldOccurrences, unmappedFields), errors);

            // Check if any valid fields were found to insert
            _validateFields.AddErrorIfNotOk(_validateFields.ValidateProspectData(prospectData), errors);

            // Validate the prospect after mapping
            _validateFields.AddErrorIfNotOk(_validateFields.ValidateProspect(prospect), errors);

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

        public object MapRequestToProspect(RequestsDTO request, List<FormFieldsModel> formFields, IMaskFormatter maskFormatter)
        {
            throw new NotImplementedException();
        }
    }
}