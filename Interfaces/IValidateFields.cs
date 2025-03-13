using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public interface IValidateFields
{
    IActionResult ValidateElecate(GenerateFormRequestDTO request);
    
    IActionResult ValidateField(FieldSaveFormRequestDTO field, string fieldName);
    
    IActionResult ValidateFieldLength(FieldSaveFormRequestDTO field, FormFieldsModel matchingField);
    
    IActionResult ValidateHoneypotFieldExists();
    
    IActionResult ValidateClientNameFieldExists();
   
    IActionResult ValidateProspectData(Dictionary<string, object> prospectData);

    IActionResult ValidateProspect(ProspectModel prospect);
}