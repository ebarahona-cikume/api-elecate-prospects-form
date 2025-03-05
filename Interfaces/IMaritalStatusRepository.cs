using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IMaritalStatusRepository
    {
        IEnumerable<MaritalStatusModel> GetAllMaritalStatuses();
    }
}
