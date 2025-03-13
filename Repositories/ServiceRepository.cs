using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Repositories
{
    public class ServiceRepository(DbContextFactory contextFactory) : IServiceRepository
    {
        private readonly DbContextFactory _dbContextFactory = contextFactory;

        public IQueryable<ServiceModel> GetAllServices()
        {
            using ElecateDbContext context = _dbContextFactory.CreateElecateDbContext();
            return context.Service_Tbl.AsQueryable();
        }
    }

}
