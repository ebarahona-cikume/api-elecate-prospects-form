using ApiElecateProspectsForm.DTOs;

namespace ApiElecateProspectsForm.Interfaces.FormFieldsGenerators
{
    public interface IFormFieldGenerator
    {
        public Task<string> GenerateComponent(FormFieldRequestDTO field);
    }
}
