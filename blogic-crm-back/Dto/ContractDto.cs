namespace blogic_crm_back.Dto
{
    public class ContractDto
    {
        public int Id { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public string? InstitutionName { get; set; }

        public DateTime DateSigned { get; set; }

        public DateTime DateValidFrom { get; set; }

        public DateTime? DateValidTo { get; set; }

        public List<UserDto> Users { get; set; } = new();
    }
}
