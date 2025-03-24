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
                // Convert the JsonElement to a JSON string and add it using the AddOriginalJsonField method
                string json = fieldElement.GetRawText();
                request.AddOriginalJsonField(json);

                // Deserialize the field and add it to the list of fields
                FieldGenerateFormRequestDTO? field = JsonSerializer.Deserialize<FieldGenerateFormRequestDTO>(json, options);
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