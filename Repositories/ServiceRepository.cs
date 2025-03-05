using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Repositories
{
    public class ServiceRepository : IServiceReository
    {
        private readonly ElecateDbContext _context;

        public ServiceRepository(ElecateDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ServiceModel> GetAllServices()
        {
            return _context.Service_Tbl.ToList();
        }
    }
}
