using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class FieldSaveFormRequestDTO
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
