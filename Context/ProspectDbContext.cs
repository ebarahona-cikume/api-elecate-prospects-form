using ApiElecateProspectsForm.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Context
{
    public class ProspectDbContext(DbContextOptions<ProspectDbContext> options) : DbContext(options)
    {
        public DbSet<ProspectModel> Prospect { get; set; }
                protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProspectModel>()
                .HasKey(p => p.pro_id);

            modelBuilder.Entity<ProspectModel>()
                .ToTable(tb => tb.UseSqlOutputClause(false))
                .Property(p => p.pro_id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ProspectModel>()
                .Property(p => p.timestamp_column)
                .IsRowVersion();

            base.OnModelCreating(modelBuilder);
        }
    }
}
