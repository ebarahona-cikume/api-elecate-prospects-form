using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Context
{
    public class SecretsDbContext(DbContextOptions<SecretsDbContext> options) : DbContext(options)
    {
        public DbSet<DbSecretsModel> VW_DB_SECRETS { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbSecretsModel>()
                .ToView("vw_db_secrets") 
                .HasNoKey();
        }
    }


}
