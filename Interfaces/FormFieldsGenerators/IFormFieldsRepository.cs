using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces.FormFieldsGenerators
{
    public interface IFormFieldsRepository
    {
        IQueryable<FormFieldsModel> GetFieldsByFormId(int Id);

        Task<List<FormFieldsModel>> GetFormFieldsAsync(int id);

        Task SyncFormFieldsAsync(int formId, IEnumerable<FormFieldsModel> newFields);
    }
}
