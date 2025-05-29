namespace blogic_crm_back.Dto
{
    public class ContractWithInstitutionDto
    {
        public string ReferenceNumber { get; set; } = string.Empty;

        public string InstitutionName { get; set; } = string.Empty;

        public DateTime DateSigned { get; set; }

        public DateTime DateValidFrom { get; set; }

        public DateTime? DateValidTo { get; set; }

        public List<int> UserIds { get; set; } = new();
    }
}
