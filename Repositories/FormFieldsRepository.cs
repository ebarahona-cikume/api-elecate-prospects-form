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

        public async Task ReplaceFieldsAsync(int idForm, IEnumerable<FormFieldsModel> newFields)
        {
            if (newFields == null || !newFields.Any())
                throw new ArgumentException("The fields list cannot be empty", nameof(newFields));

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Delete the existing fields
                    var existingFields = _context.FormFields_Tbl.Where(f => f.IdForm == idForm);
                    _context.FormFields_Tbl.RemoveRange(existingFields);
                    await _context.SaveChangesAsync();

                    // Insert the new fields
                    await _context.FormFields_Tbl.AddRangeAsync(newFields);
                    await _context.SaveChangesAsync();

                    // Confirm transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Revert changes in case of error
                    await transaction.RollbackAsync();
                    throw new Exception("An unexpected error occurred while replacing the form fields", ex);
                }
            });
        }
    }
}
