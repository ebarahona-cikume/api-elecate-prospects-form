using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Repositories
{
    public class MaritalStatusRepository : IMaritalStatusRepository
    {
        private readonly ElecateDbContext _context;

        public MaritalStatusRepository(ElecateDbContext context)
        {
            _context = context;
        }

        public IEnumerable<MaritalStatusModel> GetAllMaritalStatuses()
        {
            return _context.MaritalStatus_Tbl.ToList();
        }
    }
}