using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Models
{
    public class AccountConfirmationModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }
    }
}
