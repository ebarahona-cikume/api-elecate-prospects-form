using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<RadioOptionDTO>> GetMaritalStatusToRadioOptionsAsync()
        {
            return await _context.MaritalStatus_Tbl
                .Select(o => new RadioOptionDTO { Value = o.Id.ToString(), Label = o.MaritalStatus ?? "" })
                .ToListAsync();
        }
    }
}