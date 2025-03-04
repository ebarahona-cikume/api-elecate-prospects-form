using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class TextFieldRequestDTO: FormFieldRequestDTO
    {
        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("mask")]
        public string? Mask { get; set; }
    }
}
