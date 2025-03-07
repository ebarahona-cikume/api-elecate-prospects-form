using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class SelectFieldRequestDTO : FieldGenerateFormRequestDTO
    {
        [JsonPropertyName("relation")]
        public string? Relation { get; set; }
    }
}
