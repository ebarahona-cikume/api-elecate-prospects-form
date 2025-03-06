using ApiElecateProspectsForm.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    [JsonConverter(typeof(GenerateFormRequestConverter))]
    public class GenerateFormRequestDTO
    {
        [JsonPropertyName("fields")]
        public List<FormFieldRequestDTO?> Fields { get; set; } = [];

        [JsonIgnore]
        public List<Dictionary<string, JsonElement>> OriginalJsonFields { get; set; } = [];

        [JsonIgnore]
        private List<JsonDocument> _jsonDocuments = new();

        public void AddOriginalJsonField(string json)
        {
            var document = JsonDocument.Parse(json);
            _jsonDocuments.Add(document);
            OriginalJsonFields.Add(document.RootElement.EnumerateObject().ToDictionary(p => p.Name.ToLower(), p => p.Value));
        }
    }
}