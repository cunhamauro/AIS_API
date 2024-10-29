using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
