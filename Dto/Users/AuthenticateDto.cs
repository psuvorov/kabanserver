using System.ComponentModel.DataAnnotations;

namespace KabanServer.Dto.Users
{
    public class AuthenticateDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}