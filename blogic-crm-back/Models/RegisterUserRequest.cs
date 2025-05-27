using System.ComponentModel.DataAnnotations;

namespace blogic_crm_back.Models.Auth
{
    public class RegisterUserRequest
    {
        [Required]
        [MaxLength(255)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(5)]
        public string CountryCode { get; set; } = string.Empty;

        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(10)]
        public string SSN { get; set; } = string.Empty;

        [Required]
        [Range(1, 3, ErrorMessage = "RoleId must be 1 (client), 2 (advisor), or 3 (admin).")]
        public int RoleId { get; set; }
    }
}
