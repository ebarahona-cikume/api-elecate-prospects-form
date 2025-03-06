using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class FieldErrorDTO
    {
        [JsonPropertyOrder(1)]
        public int Index { get; set; }

        [JsonPropertyOrder(2)]
        public required List<string> FieldErrors { get; set; }
    }
}
