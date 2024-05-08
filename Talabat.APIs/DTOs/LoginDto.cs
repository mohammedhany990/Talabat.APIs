using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
    public class LoginDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        /*[RegularExpression("^(?=.* [a - z])(?=.* [A - Z])(?=.*\\d)(?=.* [^\\da - zA - Z]).{8, 15}$", 
            ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase, 1 Digit, 1 Special character")]*/
        public string Password { get; set; }
    }
}
