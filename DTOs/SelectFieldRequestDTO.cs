using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class SelectFieldRequestDTO : FormFieldRequestDTO
    {
        [JsonPropertyName("relation")]
        public string? Relation { get; set; }
    }
}
