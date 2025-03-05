using ApiElecateProspectsForm.DTOs;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class GenerateFormRequestConverter : JsonConverter<GenerateFormRequestDTO>
    {
        public override GenerateFormRequestDTO? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            GenerateFormRequestDTO request = new();

            foreach (JsonElement fieldElement in root.GetProperty("fields").EnumerateArray())
            {
                request.OriginalJsonFields.Add(fieldElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value));

                FormFieldRequestDTO? field = JsonSerializer.Deserialize<FormFieldRequestDTO>(fieldElement.GetRawText(), options);
                request.Fields.Add(field);
            }

            return request;
        }

        public override void Write(Utf8JsonWriter writer, GenerateFormRequestDTO value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

}
