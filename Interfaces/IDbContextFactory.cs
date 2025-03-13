using ApiElecateProspectsForm.Context;

namespace ApiElecateProspectsForm.Interfaces
{
    public interface IDbContextFactory
    {
        ElecateDbContext CreateElecateDbContext();
        ProspectDbContext CreateProspectDbContext(string clientDatabaseId);
    }

}
