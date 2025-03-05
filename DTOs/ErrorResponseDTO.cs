using System.Net;

namespace ApiElecateProspectsForm.DTOs
{
    public class ErrorResponseDTO
    {
        public HttpStatusCode Status { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
    }
}
