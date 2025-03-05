using ApiElecateProspectsForm.DTOs;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class FormFieldRequestConverter : JsonConverter<FormFieldRequestDTO>
    {
        public override FormFieldRequestDTO? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("type", out JsonElement typeElement))
            {
                throw new JsonException("The field 'type' is required");
            }

            string? type = typeElement.GetString();
            return type switch
            {
                "Text" or "Number" or "Password" or "Email" => JsonSerializer.Deserialize<TextFieldRequestDTO>(root.GetRawText(), options),
                "Select" or "Radio" or "Checkbox" => JsonSerializer.Deserialize<SelectFieldRequestDTO>(root.GetRawText(), options),
                _ => throw new JsonException($"Field type '{type}' is not valid"),
            };
        }

        public override void Write(Utf8JsonWriter writer, FormFieldRequestDTO value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
