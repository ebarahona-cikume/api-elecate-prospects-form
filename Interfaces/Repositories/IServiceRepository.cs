using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces.Repositories
{
    public interface IServiceRepository
    {
        IQueryable<ServiceModel> GetAllServices();
    }
}
