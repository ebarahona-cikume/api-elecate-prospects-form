using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class RadioFieldRequestDTO : FormFieldRequestDTO
    {
        [JsonPropertyName("relation")]
        public string? Relation { get; set; }
        
        [JsonPropertyName("options")]
        public List<RadioOptionDTO>? Options { get; set; }
    }

    public class RadioOptionDTO
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }
}
