using ApiElecateProspectsForm.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Context
{
    public class ProspectDbContext(DbContextOptions<ProspectDbContext> options) : DbContext(options)
    {
        public DbSet<ProspectModel> Prospect { get; set; }
    }
}
