using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces.Repositories
{
    public interface IFormFieldsRepository
    {
        IQueryable<FormFieldsModel> GetFieldsByFormId(int Id);

        Task SyncFormFieldsAsync(int formId, IEnumerable<FormFieldsModel> newFields);
    }
}
