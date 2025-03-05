using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Repositories
{
    public class ServiceRepository(ElecateDbContext context) : IServiceReository
    {
        private readonly ElecateDbContext _context = context;

        public IEnumerable<ServiceModel> GetAllServices()
        {
            return [.. _context.Service_Tbl];
        }
    }
}
