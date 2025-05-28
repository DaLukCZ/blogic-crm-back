using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blogic_crm_back.Models;

public class Contract
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(Institution))]
    public int InstitutionId { get; set; }

    public Institution? Institution { get; set; }

    [Required]
    public DateTime DateSigned { get; set; }

    [Required]
    public DateTime DateValidFrom { get; set; }

    public DateTime? DateValidTo { get; set; }

    public ICollection<ContractUser> Users { get; set; } = new List<ContractUser>();

    [NotMapped]
    public List<int> UserIds { get; set; } = new();

    [NotMapped]
    public IEnumerable<User> Clients => Users
        .Where(cu => cu.User != null && cu.User.Role?.Name == "klient")
        .Select(cu => cu.User!);

    [NotMapped]
    public IEnumerable<User> Advisors => Users
        .Where(cu => cu.User != null && cu.User.Role?.Name == "poradce")
        .Select(cu => cu.User!);
}
