using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IValidateFields
    {
        IActionResult ValidateElecate(GenerateFormRequestDTO request);

        IActionResult ValidateClientNameHoneypotFieldsExist(SaveFormDataRequestDTO request);

        bool ValidateClientNameExists(List<FieldGenerateFormRequestDTO> fields);

        bool ValidateClientNameExists(List<FieldSaveFormRequestDTO> fields);

        void ValidateFieldLength(FieldSaveFormRequestDTO field, FormFieldsModel matchingField, int index, List<FieldErrorDTO> errors);

        IActionResult ValidateProspectData(Dictionary<string, object> prospectData);

        IActionResult ValidateProspect(ProspectModel prospect);

        IActionResult ValidateUnmappedAndDuplicatedFields(List<FieldErrorDTO> repeatedFields, List<FieldErrorDTO> unmappedFields);

        IActionResult ValidateInitialRequest(SaveFormDataRequestDTO request);

        void AddErrorIfNotOk(IActionResult validationResult, List<FieldErrorDTO> errors, string index = "N/A");
    }
}