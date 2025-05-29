namespace blogic_crm_back.Dto
{
    public class InstitutionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ContractDto> Contracts { get; set; } = new();

    }

}
