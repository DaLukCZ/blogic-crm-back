using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blogic_crm_back.Models;

// Join table for many-to-many relationship between contracts and advisors
public class ContractAdvisor
{
    [Required]
    public int ContractId { get; set; }

    [ForeignKey(nameof(ContractId))]
    public Contract? Contract { get; set; }

    [Required]
    public int AdvisorId { get; set; }

    [ForeignKey(nameof(AdvisorId))]
    public User? Advisor { get; set; }
}
