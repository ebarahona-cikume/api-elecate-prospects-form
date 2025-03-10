using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IMaritalStatusRepository
    {
        Task<IEnumerable<MaritalStatusModel>> GetAllMaritalStatusesAsync();
    }
}
