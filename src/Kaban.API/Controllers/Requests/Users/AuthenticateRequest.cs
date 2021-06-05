using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Requests.Users
{
    public class AuthenticateRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}