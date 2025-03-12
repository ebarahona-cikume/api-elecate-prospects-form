using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces.Repositories;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Repositories
{
    public class ServiceRepository(DbContextFactory contextFactory) : IServiceRepository
    {
        private readonly DbContextFactory _contextFactory = contextFactory;

        public IQueryable<ServiceModel> GetAllServices()
        {
            using var context = _contextFactory.CreateElecateDbContext();
            return context.Service_Tbl.AsQueryable();
        }
    }

}
