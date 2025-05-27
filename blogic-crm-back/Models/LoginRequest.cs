using System.ComponentModel.DataAnnotations;

namespace blogic_crm_back.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(255)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;
    }
}
