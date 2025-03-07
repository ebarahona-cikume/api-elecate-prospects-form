using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class FormFieldsRepository(ElecateDbContext context) : IFormFieldsRepository
    {
        private readonly ElecateDbContext _context = context;
        public IQueryable<FormFieldsModel> GetFieldsByFormId(int Id)
        {
            return _context.FormFields_Tbl.Where(f => f.IdForm == Id);
        }

        public async Task AddFieldsAsync(IEnumerable<FormFieldsModel> fields)
        {
            if (fields == null || !fields.Any())
                throw new ArgumentException("The fields list cannot be empty", nameof(fields));

            try
            {
                await _context.FormFields_Tbl.AddRangeAsync(fields);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error inserting fields into the database", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while saving fields", ex);
            }
        }
    }
}
