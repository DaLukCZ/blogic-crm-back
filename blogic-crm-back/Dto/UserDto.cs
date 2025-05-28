using System.ComponentModel.DataAnnotations;

namespace blogic_crm_back.Dto
{
    public class UserDto
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

        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;
    }
}
