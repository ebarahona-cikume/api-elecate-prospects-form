using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IFormFieldsRepository
    {
        IQueryable<FormFieldsModel> GetFieldsByFormId(int Id);

        Task ReplaceFieldsAsync(int idForm, IEnumerable<FormFieldsModel> newFields);
    }
}
