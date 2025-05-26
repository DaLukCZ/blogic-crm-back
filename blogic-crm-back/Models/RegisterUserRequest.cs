namespace blogic_crm_back.Models.Auth
{
    public class RegisterUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public int RoleId { get; set; } // ID role: 1=client, 2=advisor, 3=admin
    }
}
