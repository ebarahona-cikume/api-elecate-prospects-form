using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class ArrayErrorMessageResponseDTO: ErrorResponseDTO
    {
        [JsonPropertyOrder(3)]
        public required List<FieldErrorDTO> Errors { get; set; }
    }
}
