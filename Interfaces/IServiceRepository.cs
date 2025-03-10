using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IServiceRepository
    {
        IQueryable<ServiceModel> GetAllServices();
    }
}
