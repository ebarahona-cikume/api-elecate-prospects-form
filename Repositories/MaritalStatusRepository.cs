using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces.Repositories;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class MaritalStatusRepository : IMaritalStatusRepository
    {
        private readonly DbContextFactory _contextFactory;

        public MaritalStatusRepository(DbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<MaritalStatusModel>> GetAllMaritalStatusesAsync()
        {
            try
            {
                using var context = _contextFactory.CreateElecateDbContext();
                return await context.MaritalStatus_Tbl.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving marital statuses.", ex);
            }
        }
    }

}