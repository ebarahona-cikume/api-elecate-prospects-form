using ApiElecateProspectsForm.Controllers;
using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            Dictionary<string, object> prospectData = [];

            // Reset the state of HoneypotFieldExists
            GlobalStateDTO.HoneypotFieldExists = false;
            GlobalStateDTO.ClientNameExists = false;

            if (request.Fields == null || !request.Fields.Any())
            {
                return _responseHandler.HandleError("Must send at least one Field.", HttpStatusCode.BadRequest);
            }

            foreach (FieldSaveFormRequestDTO field in request.Fields)
            {
                if (!GlobalStateDTO.HoneypotFieldExists || !GlobalStateDTO.ClientNameExists)
                {
                    // Honeypot validation
                    IActionResult honeypotValidationResult = _validateFields.ValidateField(field, "Honeypot");

                    if (honeypotValidationResult is not OkResult)
                    {
                        return honeypotValidationResult;
                    }

                    // ClientName validation
                    IActionResult clientValidationResult = _validateFields.ValidateField(field, "ClientName");

                    if (clientValidationResult is not OkResult)
                    {
                        return clientValidationResult;
                    }
                }

                FormFieldsModel? matchingField = formFields.FirstOrDefault(f => f.Name != null && f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

                if (matchingField != null)
                {
                    IActionResult lengthValidationResult = _validateFields.ValidateFieldLength(field, matchingField);
                    if (lengthValidationResult is not OkResult)
                    {
                        return lengthValidationResult;
                    }

                    // Apply the mask if it exists
                    string fieldValue = field.Value ?? "";

                    if (!string.IsNullOrEmpty(matchingField.Mask))
                    {
                        fieldValue = maskFormatter.ApplyMask(fieldValue, matchingField.Mask);
                    }

                    if (!string.IsNullOrEmpty(field.Name))
                    {
                        prospectData[field.Name] = fieldValue;  // Assign the masked value
                    }
                }
            }

            // Check if honeypot field exists
            IActionResult honeypotFieldExistsResult = _validateFields.ValidateHoneypotFieldExists();
            if (honeypotFieldExistsResult is not OkResult)
            {
                return honeypotFieldExistsResult;
            }

            // Check if ClientName field exists
            IActionResult clientNameFieldExistsResult = _validateFields.ValidateClientNameFieldExists();
            if (clientNameFieldExistsResult is not OkResult)
            {
                return clientNameFieldExistsResult;
            }

            // Check if any valid fields were found to insert
            IActionResult prospectDataValidationResult = _validateFields.ValidateProspectData(prospectData);
            if (prospectDataValidationResult is not OkResult)
            {
                return prospectDataValidationResult;
            }


            foreach ((KeyValuePair<string, object> entry, System.Reflection.PropertyInfo property) in
            // Map the dictionary to the ProspectModel properties
            from KeyValuePair<string, object> entry in prospectData
            let property = typeof(ProspectModel).GetProperty(entry.Key,
                                System.Reflection.BindingFlags.IgnoreCase |
                                System.Reflection.BindingFlags.Public |
                                System.Reflection.BindingFlags.Instance)
            where property != null && property.CanWrite
            select (entry, property))
            {
                property.SetValue(prospect, Convert.ChangeType(entry.Value, property.PropertyType));
            }

            // Validate the prospect after mapping
            IActionResult validateProspectResult = _validateFields.ValidateProspect(prospect);
            return validateProspectResult is OkResult ? new OkObjectResult(prospect) : (object)validateProspectResult;
        }
    }
}