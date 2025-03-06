using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;

namespace ApiElecateProspectsForm.Services.FormFieldsGenerators
{
    public class CheckboxFieldGenerator : IFormFieldGenerator
    {
        public Task<string> GenerateComponent(FormFieldRequestDTO field)
        {
            throw new NotImplementedException();
        }
    }
}
