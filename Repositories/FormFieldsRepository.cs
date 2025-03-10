using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class FormFieldsRepository(DbContextFactory contextFactory) : IFormFieldsRepository
    {
        private readonly DbContextFactory _contextFactory = contextFactory;
        public IQueryable<FormFieldsModel> GetFieldsByFormId(int Id)
        {
            var dbContext = _contextFactory.CreateElecateDbContext(); // Crear un nuevo DbContext dinámico

            return dbContext.FormFields_Tbl
                .Where(f => f.IdForm == Id && !f.IsDeleted); // Excluir registros eliminados lógicamente
        }


        public async Task SyncFormFieldsAsync(int formId, IEnumerable<FormFieldsModel> newFields)
        {
            ArgumentNullException.ThrowIfNull(newFields);

            var executionStrategy = _contextFactory.CreateElecateDbContext().Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var dbContext = _contextFactory.CreateElecateDbContext(); // Create a new instance
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                try
                {
                    var existingFields = await dbContext.FormFields_Tbl
                        .Where(f => f.IdForm == formId)
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

                    // Mark fields to be deleted instead of removing them
                    foreach (var field in fieldsToRemove)
                    {
                        field.IsDeleted = true; // Logical delete
                        dbContext.FormFields_Tbl.Update(field);
                    }

                    // Update fields that have changes
                    foreach (var field in fieldsToUpdate)
                    {
                        var newField = newFields.First(f => f.Name == field.Name);

                        if (field.Type != newField.Type ||
                            field.Size != newField.Size ||
                            field.Mask != newField.Mask ||
                            field.Link != newField.Link ||
                            field.Relation != newField.Relation)
                        {
                            field.Type = newField.Type;
                            field.Size = newField.Size;
                            field.Mask = newField.Mask;
                            field.Link = newField.Link;
                            field.Relation = newField.Relation;
                            dbContext.FormFields_Tbl.Update(field);
                        }
                    }

                    // Add new fields
                    if (fieldsToAdd.Any())
                        await dbContext.FormFields_Tbl.AddRangeAsync(fieldsToAdd);

                    // Save changes
                    await dbContext.SaveChangesAsync();

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
