using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IProspectMapper
    {
        ProspectResultDTO MapRequestToProspect(
                    SaveFormDataRequestDTO request,
                    List<FormFieldsModel> formFields,
                    IMaskFormatter maskFormatter);
    }
}