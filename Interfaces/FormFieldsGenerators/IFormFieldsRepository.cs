using ApiElecateProspectsForm.Models;

<<<<<<<< HEAD:Interfaces/Repositories/IFormFieldsRepository.cs
namespace ApiElecateProspectsForm.Interfaces.Repositories
========
namespace ApiElecateProspectsForm.Interfaces.FormFieldsGenerators
>>>>>>>> sdiaz:Interfaces/FormFieldsGenerators/IFormFieldsRepository.cs
{
    public interface IFormFieldsRepository
    {
        IQueryable<FormFieldsModel> GetFieldsByFormId(int Id);

        Task<List<FormFieldsModel>> GetFormFieldsAsync(int id);

        Task SyncFormFieldsAsync(int formId, IEnumerable<FormFieldsModel> newFields);
    }
}
