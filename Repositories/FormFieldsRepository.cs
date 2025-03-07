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

        public async Task SyncFormFieldsAsync(int formId, IEnumerable<FormFieldsModel> newFields)
        {
            ArgumentNullException.ThrowIfNull(newFields);

            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var existingFields = await _context.FormFields_Tbl
                        .Where(f => f.IdForm == formId && !f.IsDeleted) // Exclude deleted fields
                        .ToListAsync();

                    // Find fields to mark as deleted (fields in the DB but not in the new fields)
                    var fieldsToRemove = existingFields
                        .Where(existing => !newFields.Any(newField => newField.Name == existing.Name))
                        .ToList();

                    // Find fields to update (fields that exist both in DB and new data)
                    var fieldsToUpdate = existingFields
                        .Where(existing => newFields.Any(newField => newField.Name == existing.Name))
                        .ToList();

                    // Find new fields to add (fields in the new data but not in DB)
                    var fieldsToAdd = newFields
                        .Where(newField => !existingFields.Any(existing => existing.Name == newField.Name))
                        .ToList();

                    // Mark fields to be deleted as logically deleted (Set IsDeleted to true)
                    foreach (var field in fieldsToRemove)
                    {
                        field.IsDeleted = true; // Set IsDeleted to true to perform a logical delete
                    }

                    // Update fields that have changes
                    foreach (var field in fieldsToUpdate)
                    {
                        var newField = newFields.First(f => f.Name == field.Name);

                        // Only update if the field properties have changed
                        if (field.Name != newField.Name ||
                            field.Type != newField.Type ||
                            field.Size != newField.Size ||
                            field.Mask != newField.Mask ||
                            field.Link != newField.Link ||
                            field.Relation != newField.Relation)
                        {
                            field.Name = newField.Name;
                            field.Type = newField.Type;
                            field.Size = newField.Size;
                            field.Mask = newField.Mask;
                            field.Link = newField.Link;
                            field.Relation = newField.Relation;
                        }
                    }

                    // Add new fields
                    if (fieldsToAdd.Any())
                        await _context.FormFields_Tbl.AddRangeAsync(fieldsToAdd);

                    // Save changes
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Rollback transaction on error
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("An error occurred while synchronizing form fields.", ex);
                }
            });
        }
    }
}
