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

            string? type = typeElement.GetString();
            return type switch
            {
                "Text" or "Number" or "Password" or "Email" => JsonSerializer.Deserialize<TextFieldRequestDTO>(root.GetRawText(), options),
                "Select" or "Checkbox" or "Radio" => JsonSerializer.Deserialize<SelectFieldRequestDTO>(root.GetRawText(), options),
                _ => null,
            };
        }

        public override void Write(Utf8JsonWriter writer, FieldGenerateFormRequestDTO value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
