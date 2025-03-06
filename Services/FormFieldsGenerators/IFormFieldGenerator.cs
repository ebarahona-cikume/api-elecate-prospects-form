using ApiElecateProspectsForm.DTOs;
using System.Text;

namespace ApiElecateProspectsForm.Services.FormComponentsGenerators
{
    public interface IFormFieldGenerator
    {
        public Task<string> GenerateComponent(FormFieldRequestDTO field);
    }
}
