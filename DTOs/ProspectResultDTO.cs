using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.DTOs
{
    public class ProspectResultDTO
    {
        public bool Success { get; set; }
        public List<FieldErrorDTO>? Errors { get; set; }
        public ProspectModel? Prospect { get; set; }
    }
}
