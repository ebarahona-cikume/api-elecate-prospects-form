using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class MaritalStatusRepository : IMaritalStatusRepository
    {
        private readonly DbContextFactory _dbContextFactory;

        public MaritalStatusRepository(DbContextFactory contextFactory)
        {
            _dbContextFactory = contextFactory;
        }

        public async Task<IEnumerable<MaritalStatusModel>> GetAllMaritalStatusesAsync()
        {
            try
            {
                using ElecateDbContext context = _dbContextFactory.CreateElecateDbContext();
                return await context.MaritalStatus_Tbl.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving marital statuses.", ex);
            }
        }
    }

}