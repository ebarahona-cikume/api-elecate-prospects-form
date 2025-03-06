using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;

namespace ApiElecateProspectsForm.Services.FormFieldsGenerators
{
    public class TextFieldGenerator : IFormFieldGenerator
    {
        public Task<string> GenerateComponent(FormFieldRequestDTO field)
        {
            if (field is not TextFieldRequestDTO textField)
                throw new ArgumentException("Invalid field type");

            string maxLength = textField.Size > 0 ? $"maxlength=\"{textField.Size}\"" : "";
            string mask = !string.IsNullOrEmpty(textField.Mask) ? $"mask=\"{textField.Mask}\"" : "";

            return Task.FromResult($"<input type=\"{textField.Type}\" id=\"{textField.Name}\" name=\"{textField.Name}\" {maxLength} {mask} />\n");
        }
    }
}
