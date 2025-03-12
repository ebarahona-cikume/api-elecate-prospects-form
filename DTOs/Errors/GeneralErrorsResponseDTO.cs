using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs.Errors
{
    public class GeneralErrorsResponseDTO : ErrorResponseDTO
    {
        [JsonPropertyOrder(3)]
        public required List<string> Errors { get; set; }
    }
}
