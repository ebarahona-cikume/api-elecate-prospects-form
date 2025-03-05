using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IServiceReository
    {
        IEnumerable<ServiceModel> GetAllServices();
    }
}
