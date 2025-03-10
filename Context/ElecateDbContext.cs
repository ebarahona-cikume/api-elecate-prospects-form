using Microsoft.EntityFrameworkCore;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Context
{
    public class ElecateDbContext(DbContextOptions<ElecateDbContext> options) : DbContext(options)
    {
        public DbSet<MaritalStatusModel> MaritalStatus_Tbl { get; set; }
        public DbSet<ServiceModel> Service_Tbl { get; set; }
        public DbSet<FormFieldsModel> FormFields_Tbl { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new InvalidOperationException("ElecateDbContext requires a configured DbContext.");
            }
        }
    }

}