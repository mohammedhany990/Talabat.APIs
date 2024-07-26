using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be at least 6 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }



    }
}
