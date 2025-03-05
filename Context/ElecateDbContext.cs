using Microsoft.EntityFrameworkCore;
using ApiElecateProspectsForm.Models;

namespace ApiElecateProspectsForm.Context
{
    public class ElecateDbContext : DbContext
    {
        public ElecateDbContext(DbContextOptions<ElecateDbContext> options) : base(options) { }

        public DbSet<MaritalStatusModel> MaritalStatus_Tbl { get; set; }

        public DbSet<ServiceModel> Service_Tbl { get; set; }
    }

}
