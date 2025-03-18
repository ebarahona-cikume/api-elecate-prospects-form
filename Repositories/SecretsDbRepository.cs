using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces.Repositories;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class SecretsDbRepository(DbContextFactory contextFactory) : ISecretsDbRepository
    {
        private readonly DbContextFactory _dbContextFactory = contextFactory;

        public DbContextFactory GetDbContextFactory()
        {
            return _dbContextFactory;
        }

        public async Task<DbSecretsModel?> GetDbSecretsFieldsAsync(Guid id)
        {
            SecretsDbContext? secretDbContext = null;

            try
            {
                secretDbContext = GetDbContextFactory().CreateSecretsPostgresqlDbContext();
                DbSecretsModel? secret = await secretDbContext.Set<DbSecretsModel>()
                    .FromSqlRaw("SELECT * FROM VW_DB_SECRETS WHERE secret_id = {0} AND is_active = true", id)
                    .FirstOrDefaultAsync();

                return secret;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return null;
            }
            finally
            {
                if (secretDbContext != null)
                {
                    await secretDbContext.DisposeAsync();
                }
            }
        }
    }
}
