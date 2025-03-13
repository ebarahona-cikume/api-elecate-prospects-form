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

        IActionResult ValidateFieldLength(FieldSaveFormRequestDTO field, FormFieldsModel matchingField);

        IActionResult ValidateProspectData(Dictionary<string, object> prospectData);

        IActionResult ValidateProspect(ProspectModel prospect);

        IActionResult ValidateUnmappedAndDuplicatedFields(Dictionary<string, int> fieldOccurrences, List<string> unmappedFields);

        IActionResult ValidateInitialRequest(SaveFormDataRequestDTO request);

        void AddErrorIfNotOk(IActionResult validationResult, List<FieldErrorDTO> errors);
    }
}