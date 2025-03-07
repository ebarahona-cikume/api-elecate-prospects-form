using ApiElecateProspectsForm.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    [JsonConverter(typeof(GenerateFormRequestConverter))]
    public class GenerateFormRequestDTO
    {
        [JsonPropertyName("fields")]
        public List<FieldGenerateFormRequestDTO?> Fields { get; set; } = [];

        [JsonIgnore]
        public List<Dictionary<string, JsonElement>> OriginalJsonFields { get; set; } = [];

        [JsonIgnore]
        private List<JsonDocument> _jsonDocuments = [];

        public void AddOriginalJsonField(string json)
        {
            JsonDocument document = JsonDocument.Parse(json);
            _jsonDocuments.Add(document);
            OriginalJsonFields.Add(document.RootElement.EnumerateObject().ToDictionary(p => p.Name.ToLower(), p => p.Value));
        }
    }
}