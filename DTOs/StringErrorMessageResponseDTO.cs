using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class StringErrorMessageResponseDTO: ErrorResponseDTO
    {
        [JsonPropertyOrder(3)]
        public required string Message { get; set; }
    }
}
