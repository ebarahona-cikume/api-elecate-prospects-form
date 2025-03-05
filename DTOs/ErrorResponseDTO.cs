namespace ApiElecateProspectsForm.DTOs
{
    public class ErrorResponseDTO
    {
        public int Status { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
    }
}
