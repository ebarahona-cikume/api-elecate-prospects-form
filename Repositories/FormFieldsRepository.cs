using ApiElecateProspectsForm.Context;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Repositories
{
    public class FormFieldsRepository(ElecateDbContext context) : IFormFieldsRepository
    {
        private readonly ElecateDbContext _context = context;
        public IQueryable<FormFieldsModel> GetFieldsByFormId(int Id)
        {
            return _context.FormFields_Tbl.Where(f => f.IdForm == Id);
        }
    }
}
