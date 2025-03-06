using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IServiceRepository
    {
        IEnumerable<ServiceModel> GetAllServices();
    }
}
