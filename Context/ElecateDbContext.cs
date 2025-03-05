using Microsoft.EntityFrameworkCore;
using ApiElecateProspectsForm.DTOs;

namespace ApiElecateProspectsForm.Context
{
    public class ElecateDbContext : DbContext
    {
        public ElecateDbContext(DbContextOptions<ElecateDbContext> options) : base(options) { }

        public DbSet<MaritalStatusDTO> MaritalStatus_Tbl { get; set; }

        public DbSet<ServiceDTO> Service_Tbl { get; set; }
    }

}
