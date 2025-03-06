using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Services.FormComponentsGenerators;
using System.Text;

namespace ApiElecateProspectsForm.Services.FormFieldsGenerators
{
    public class RadioFieldGenerator : IFormFieldGenerator
    {
        private readonly IMaritalStatusRepository _maritalStatusRepository;

        public RadioFieldGenerator(IMaritalStatusRepository maritalStatusRepository)
        {
            _maritalStatusRepository = maritalStatusRepository;
        }

        public async Task<string> GenerateComponent(FormFieldRequestDTO field)
        {
            if (field is not RadioFieldRequestDTO radioField)
                throw new ArgumentException("Invalid field type");

            List<RadioOptionDTO> options;

            if (!string.IsNullOrEmpty(radioField.Relation))
            {
                options = await _maritalStatusRepository.GetMaritalStatusToRadioOptionsAsync();
            }
            else if (radioField.Options != null && radioField.Options.Count != 0)
            {
                // Usa las opciones definidas en el JSON
                options = radioField.Options;
            }
            else
            {
                throw new InvalidOperationException("Radio field must have either a relation or defined options.");
            }

            // Construcción del HTML
            var htmlBuilder = new StringBuilder();
            foreach (var option in options)
            {
                htmlBuilder.Append($"<input type=\"radio\" id=\"{option.Value}\" name=\"{field.Name}\" value=\"{option.Value}\">\n");
                htmlBuilder.Append($"<label for=\"{option.Value}\">{option.Label.Trim()}</label>\n");
            }

            return htmlBuilder.ToString();
        }
    }
}
