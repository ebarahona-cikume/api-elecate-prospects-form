using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IFormFieldsRepository
    {
        IQueryable<FormFieldsModel> GetFieldsByFormId(int Id);

        Task SyncFormFieldsAsync(int formId, IEnumerable<FormFieldsModel> newFields);
    }
}
