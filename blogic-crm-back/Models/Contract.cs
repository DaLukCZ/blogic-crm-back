using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blogic_crm_back.Models;

// Client's contract managed by an advisor
public class Contract
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string ReferenceNumber { get; set; } = string.Empty;

    // Foreign key to Institution
    [Required]
    [ForeignKey(nameof(Institution))]
    public int InstitutionId { get; set; }

    public Institution? Institution { get; set; }

    [Required]
    public DateTime DateSigned { get; set; }

    [Required]
    public DateTime DateValidFrom { get; set; }

    public DateTime? DateValidTo { get; set; }

    // Foreign key to Client (User)
    [Required]
    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }

    public User? Client { get; set; }

    // Foreign key to Manager (User)
    [Required]
    [ForeignKey(nameof(Manager))]
    public int ManagerId { get; set; }

    public User? Manager { get; set; }

    public ICollection<ContractAdvisor>? ContractAdvisors { get; set; }
}
