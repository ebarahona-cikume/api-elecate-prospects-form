using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;
using ApiElecateProspectsForm.Services.FormComponentsGenerators;

namespace ApiElecateProspectsForm.Services.FormFieldsGenerators
{
    public class FieldGeneratorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FieldGeneratorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFormFieldGenerator GetGenerator(FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.Text => _serviceProvider.GetRequiredService<TextFieldGenerator>(),
                FieldType.Number => _serviceProvider.GetRequiredService<TextFieldGenerator>(),
                FieldType.Password => _serviceProvider.GetRequiredService<TextFieldGenerator>(),
                FieldType.Email => _serviceProvider.GetRequiredService<TextFieldGenerator>(),
                FieldType.Select => _serviceProvider.GetRequiredService<SelectFieldGenerator>(),
                FieldType.Radio => _serviceProvider.GetRequiredService<RadioFieldGenerator>(),
                FieldType.Checkbox => _serviceProvider.GetRequiredService<CheckboxFieldGenerator>(),
                _ => throw new NotImplementedException($"No generator found for {fieldType}")
            };
        }
    }
}
