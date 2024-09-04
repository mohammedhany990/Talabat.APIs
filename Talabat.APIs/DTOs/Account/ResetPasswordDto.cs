using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "Password doesn't match")]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string ConfirmedNewPassword { get; set; }


        public string Token { get; set; }
    }
}
