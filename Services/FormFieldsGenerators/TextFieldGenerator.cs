using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;
using ApiElecateProspectsForm.Utils;

namespace ApiElecateProspectsForm.Services.FormFieldsGenerators
{
    public class TextFieldGenerator : IFormFieldGenerator
    {
        private readonly ResponseHandler _responseHandler = new();

        public Task<string> GenerateComponent(FieldGenerateFormRequestDTO field)
        {
            if (field is not TextFieldRequestDTO textField)
            {
                // Handle the error by throwing an exception with the error message
                _ = _responseHandler.HandleError("Invalid field type");
                throw new ArgumentException("Invalid field type");
            }

            string maxLength = textField.Size > 0 ? $"maxlength=\"{textField.Size}\"" : "";
            string mask = !string.IsNullOrEmpty(textField.Mask) ? $"mask=\"{textField.Mask}\"" : "";

            return Task.FromResult($"<input type=\"{textField.Type}\" id=\"{textField.Name}\" name=\"{textField.Name}\" {maxLength} {mask} />\n");
        }
    }
}
