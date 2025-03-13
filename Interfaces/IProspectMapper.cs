using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Controllers
{
    public interface IProspectMapper
    {
        object MapRequestToProspect(
                    SaveFormDataRequestDTO request,
                    List<FormFieldsModel> formFields,
                    IMaskFormatter maskFormatter);
    }
}