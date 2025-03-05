using ApiElecateProspectsForm.DTOs;
using System.Text;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IGenerateFormComponent
    {
        public void GenerateComponent(StringBuilder htmlBuilder, FormFieldRequestDTO field);
    }
}
