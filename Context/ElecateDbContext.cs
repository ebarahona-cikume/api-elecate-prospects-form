using Microsoft.EntityFrameworkCore;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Context
{
    public class ElecateDbContext : DbContext
    {
        private readonly string _connectionString = string.Empty;

        public ElecateDbContext(DbContextOptions<ElecateDbContext> options)
            : base(options)
        {
        }

        public ElecateDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public DbSet<MaritalStatusModel> MaritalStatus_Tbl { get; set; }

        public DbSet<ServiceModel> Service_Tbl { get; set; }

        public DbSet<FormFieldsModel> FormFields_Tbl { get; set; }
    }

}