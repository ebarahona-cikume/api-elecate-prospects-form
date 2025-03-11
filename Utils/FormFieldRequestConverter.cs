using ApiElecateProspectsForm.DTOs;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class FormFieldRequestConverter : JsonConverter<FieldGenerateFormRequestDTO>
    {
        public override FieldGenerateFormRequestDTO? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("type", out JsonElement typeElement))
            {
                return null;
            }

            string? type = typeElement.GetString()?.ToLowerInvariant() ?? "";
            List<string> textFieldTypes = ["text", "number", "password", "email"];
            List<string> selectFieldTypes = ["select", "checkbox", "radio"];

            if (textFieldTypes.Contains(type))
            {
                return JsonSerializer.Deserialize<TextFieldRequestDTO>(root.GetRawText(), options);
            }
            else if (selectFieldTypes.Contains(type))
            {
                return JsonSerializer.Deserialize<SelectFieldRequestDTO>(root.GetRawText(), options);
            }
            else
            {
                return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, FieldGenerateFormRequestDTO value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
