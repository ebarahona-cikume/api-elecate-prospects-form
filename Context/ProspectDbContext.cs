using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Context
{
    public class ProspectDbContext(string connectionString) : DbContext
    {
        private readonly string _connectionString = connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public DbSet<ProspectModel> Prospect { get; set; }
    }
}
