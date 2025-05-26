using System.ComponentModel.DataAnnotations;

namespace blogic_crm_back.Models;

// Uživatelská role (např. "client", "advisor", "admin")
public class Role
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<User>? Users { get; set; }
}
