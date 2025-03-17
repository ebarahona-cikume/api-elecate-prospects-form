using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces.Repositories
{
    public interface ISecretsDbRepository
    {
        Task<DbSecretsModel?> GetDbSecretsFieldsAsync(Guid id);
    }
}