namespace blogic_crm_back.Models;

// Spojovací tabulka smlouvy a poradců (M:N)
public class ContractAdvisor
{
    public int ContractId { get; set; }
    public Contract? Contract { get; set; }

    public int AdvisorId { get; set; }
    public User? Advisor { get; set; }
}
