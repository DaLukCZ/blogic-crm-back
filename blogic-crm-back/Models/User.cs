using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace blogic_crm_back.Models;

// Uživatel (klient, poradce, admin...)
public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? CountryCode { get; set; }
    public string? Number { get; set; }
    public string? SSN { get; set; }
    public DateTime? BirthDate { get; set; }

    // Smlouvy kde je klient
    public ICollection<Contract>? ClientContracts { get; set; }

    // Smlouvy kde je správce (hlavní poradce)
    public ICollection<Contract>? ManagedContracts { get; set; }

    // Další účast v rámci smluv (M:N)
    public ICollection<ContractAdvisor>? ContractAdvisors { get; set; }
}
