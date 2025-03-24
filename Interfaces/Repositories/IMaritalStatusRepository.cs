using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces.Repositories
{
    public interface IMaritalStatusRepository
    {
        Task<IEnumerable<MaritalStatusModel>> GetAllMaritalStatusesAsync();
    }
}
