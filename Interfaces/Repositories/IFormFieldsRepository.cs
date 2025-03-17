using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces.Repositories
{
    public interface IFormFieldsRepository
    {
        IQueryable<FormFieldsModel> GetFieldsByFormId(Guid Id);

        Task<List<FormFieldsModel>> GetFormFieldsAsync(Guid id);

        Task SyncFormFieldsAsync(Guid formId, IEnumerable<FormFieldsModel> newFields);
    }
}
