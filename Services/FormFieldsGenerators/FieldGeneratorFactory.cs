using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;
using ApiElecateProspectsForm.Services.FormComponentsGenerators;
using ApiElecateProspectsForm.Services.FormFieldsGenerators;

public class FieldGeneratorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<FieldType, Type> _generatorMap;

    public FieldGeneratorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _generatorMap = new Dictionary<FieldType, Type>
        {
            { FieldType.Text, typeof(TextFieldGenerator) },
            { FieldType.Number, typeof(TextFieldGenerator) },
            { FieldType.Password, typeof(TextFieldGenerator) },
            { FieldType.Email, typeof(TextFieldGenerator) },
            { FieldType.Select, typeof(SelectFieldGenerator) },
            { FieldType.Radio, typeof(RadioFieldGenerator) },
            { FieldType.Checkbox, typeof(CheckboxFieldGenerator) }
        };
    }

    public IFormFieldGenerator GetGenerator(FieldType fieldType)
    {
        if (_generatorMap.TryGetValue(fieldType, out var generatorType))
        {
            return (IFormFieldGenerator)_serviceProvider.GetRequiredService(generatorType);
        }
        throw new NotImplementedException($"No generator found for {fieldType}");
    }

    public void AddGenerator(FieldType fieldType, Type generatorType)
    {
        _generatorMap[fieldType] = generatorType;
    }
}