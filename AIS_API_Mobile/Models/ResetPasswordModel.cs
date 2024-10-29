using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Models
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required]
        [MinLength(6)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The New Password and Confirmation Password do not match.")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
