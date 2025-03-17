using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces.Repositories;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class SecretsDbRepository(DbContextFactory contextFactory) : ISecretsDbRepository
    {
        private readonly DbContextFactory _dbContextFactory = contextFactory;

        //public async Task<DbSecretsModel?> GetDbSecretsFieldsAsync(Guid id)
        //{
        //    await using SecretsDbContext secretDbContext = _dbContextFactory.CreateSecretsPostgresqlDbContext();
        //    DbSecretsModel? secret = await secretDbContext.VW_DB_SECRETS
        //        .Where(f => f.SecretId == id && f.IsActive)
        //        .FirstOrDefaultAsync();

        //    return secret;
        //}
        public async Task<DbSecretsModel?> GetDbSecretsFieldsAsync(Guid id)
        {
            SecretsDbContext? secretDbContext = null;
            try
            {
                secretDbContext = _dbContextFactory.CreateSecretsPostgresqlDbContext();
                var secret = await secretDbContext.Set<DbSecretsModel>()
                    .FromSqlRaw("SELECT * FROM VW_DB_SECRETS WHERE secret_id = {0} AND is_active = true", id)
                    .FirstOrDefaultAsync();

                return secret;
            }
            catch (Exception ex)
            {
                // Registrar el mensaje de error y la pila de llamadas
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
