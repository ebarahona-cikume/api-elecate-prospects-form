using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs
{
    public class RequestsDTO
    {

    [JsonIgnore]
    private List<JsonDocument> _jsonDocuments = new List<JsonDocument>();
        public List<Dictionary<string, JsonElement>>? Fields { get; set; }
        public object? OriginalJsonFields { get; internal set; }
    }
}
