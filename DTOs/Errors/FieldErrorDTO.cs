using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs.Errors
{
    public class FieldErrorDTO
    {
        [JsonPropertyOrder(1)]
        public required string Index { get; set; }

        [JsonPropertyOrder(2)]
        public required List<string> FieldErrors { get; set; }
    }
}
