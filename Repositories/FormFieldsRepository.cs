using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces.Repositories;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class FormFieldsRepository(DbContextFactory contextFactory) : IFormFieldsRepository
    {
        private readonly DbContextFactory _dbContextFactory = contextFactory;

        public IQueryable<FormFieldsModel> GetFieldsByFormId(int Id)
        {
            ElecateDbContext dbContext = _dbContextFactory.CreateElecateDbContext(); // Create a new dynamic DbContext

            return dbContext.FormFields_Tbl
                .Where(f => f.IdForm == Id && !f.IsDeleted); // Exclude logically deleted records
        }

        public async Task<List<FormFieldsModel>> GetFormFieldsAsync(int id)
        {
            await using ElecateDbContext elecateDbContext = _dbContextFactory.CreateElecateDbContext();
            return await elecateDbContext.FormFields_Tbl
                .Where(f => f.IdForm == id && !f.IsDeleted)
                .ToListAsync();
        }

        public async Task SyncFormFieldsAsync(int formId, IEnumerable<FormFieldsModel> newFields)
        {
            ArgumentNullException.ThrowIfNull(newFields);

            Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy executionStrategy = _dbContextFactory.CreateElecateDbContext().Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                await using ElecateDbContext dbContext = _dbContextFactory.CreateElecateDbContext(); // Create a new instance
                await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();

                try
                {
                    List<FormFieldsModel> existingFields = await dbContext.FormFields_Tbl
                        .Where(f => f.IdForm == formId && !f.IsDeleted).ToListAsync();

                    // Find fields to mark as deleted (fields in the DB but not in the new fields)
                    List<FormFieldsModel> fieldsToRemove = [.. existingFields.Where(existing => !newFields.Any(newField => newField.Name == existing.Name))];

                    // Find fields to update (fields that exist both in DB and new data)
                    List<FormFieldsModel> fieldsToUpdate = [.. existingFields.Where(existing => newFields.Any(newField => newField.Name == existing.Name))];

                    // Find new fields to add (fields in the new data but not in DB)
                    List<FormFieldsModel> fieldsToAdd = [.. newFields.Where(newField => !existingFields.Any(existing => existing.Name == newField.Name))];

                    // Mark fields to be deleted instead of removing them
                    foreach (FormFieldsModel? field in fieldsToRemove)
                    {
                        field.IsDeleted = true; // Logical delete
                        dbContext.FormFields_Tbl.Update(field);
                    }

                    // Update fields that have changes
                    foreach (FormFieldsModel? field in fieldsToUpdate)
                    {
                        FormFieldsModel newField = newFields.First(f => f.Name == field.Name);

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
                    if (fieldsToAdd.Count != 0)
                    {
                        await dbContext.FormFields_Tbl.AddRangeAsync(fieldsToAdd);
                    }

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