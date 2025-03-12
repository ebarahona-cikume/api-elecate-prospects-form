using System.Net;
using System.Text.Json.Serialization;

namespace ApiElecateProspectsForm.DTOs.Errors
{
    public abstract class ErrorResponseDTO
    {
        [JsonPropertyOrder(1)]
        public HttpStatusCode Status { get; set; }

        [JsonPropertyOrder(2)]
        public required string Title { get; set; }
    }
}
