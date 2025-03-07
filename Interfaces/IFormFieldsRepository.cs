using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IFormFieldsRepository
    {
        IQueryable<FormFieldsModel> GetFieldsByFormId(int Id);

        Task AddFieldsAsync(IEnumerable<FormFieldsModel> fields);
    }
}
