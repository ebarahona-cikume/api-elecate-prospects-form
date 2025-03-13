using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs.Errors
{
<<<<<<<< HEAD:DTOs/Errors/FormFieldErrorsMessagesResponseDTO.cs
    public class FormFieldErrorsMessagesResponseDTO : ErrorResponseDTO
========
    public class FormFieldErrorMessagesResponseDTO : ErrorResponseDTO
>>>>>>>> sdiaz:DTOs/Errors/FormFieldErrorMessagesResponseDTO.cs
    {
        [JsonPropertyOrder(3)]
        public required List<FieldErrorDTO> Errors { get; set; }
    }
}
