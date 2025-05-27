using System.ComponentModel.DataAnnotations;

namespace blogic_crm_back.Models.Auth
{
    public class LoginResponse
    {
        [Required]
        [MaxLength(2000)]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Username { get; set; } = string.Empty;
    }
}
