using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Models
{
    public class UpdateUserModel
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
