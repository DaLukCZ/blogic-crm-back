using System.ComponentModel.DataAnnotations;

namespace blogic_crm_back.Models;

// Smlouva klienta se správou poradce
public class Contract
{
    public int Id { get; set; }

    [Required]
    public string ReferenceNumber { get; set; } = string.Empty;

    [Required]
    public string Institution { get; set; } = string.Empty;

    public DateTime DateSigned { get; set; }
    public DateTime DateValidFrom { get; set; }
    public DateTime? DateValidTo { get; set; }

    // FK na klienta
    public int ClientId { get; set; }
    public User? Client { get; set; }

    // FK na hlavního správce smlouvy (poradce)
    public int ManagerId { get; set; }
    public User? Manager { get; set; }

    public ICollection<ContractAdvisor>? ContractAdvisors { get; set; }
}
