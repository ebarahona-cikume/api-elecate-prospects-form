using Microsoft.EntityFrameworkCore;

namespace ApiElecateProspectsForm.Models;

[Keyless]
public class DbSecretsModel
{
    public required Guid secret_id { get; set; }

    public string? db_name { get; set; }

    public bool is_active { get; set; }
}
