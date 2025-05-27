using System.ComponentModel.DataAnnotations;

namespace blogic_crm_back.Models;

// User (client, advisor, admin, etc.)
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    public Role? Role { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(5)]
    public string? CountryCode { get; set; }

    [MaxLength(15)]
    public string? Number { get; set; }

    [MaxLength(10)]
    public string? SSN { get; set; }

    // Contracts where the user is a client
    public ICollection<Contract>? ClientContracts { get; set; }

    // Contracts where the user is the manager (main advisor)
    public ICollection<Contract>? ManagedContracts { get; set; }

    // Additional participation as advisor (many-to-many)
    public ICollection<ContractAdvisor>? ContractAdvisors { get; set; }
}
