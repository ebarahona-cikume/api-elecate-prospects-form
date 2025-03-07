using ApiElecateProspectsForm.Utils;
using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public enum FieldType
    {
        Text,
        Number,
        Password,
        Email,
        Select,
        Radio,
        Checkbox,
    }

    [JsonConverter(typeof(FormFieldRequestConverter))]
    public abstract class FieldGenerateFormRequestDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("link")]
        public int Link { get; set; }
    }
}
