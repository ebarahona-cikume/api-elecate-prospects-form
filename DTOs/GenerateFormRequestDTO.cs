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
    }
}
