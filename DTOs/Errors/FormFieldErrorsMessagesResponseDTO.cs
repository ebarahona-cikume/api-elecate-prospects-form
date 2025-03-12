using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs.Errors
{
    public class FormFieldErrorsMessagesResponseDTO : ErrorResponseDTO
    {
        [JsonPropertyOrder(3)]
        public required List<FieldErrorDTO> Errors { get; set; }
    }
}
