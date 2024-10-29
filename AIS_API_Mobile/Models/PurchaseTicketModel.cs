using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Models
{
    public class PurchaseTicketModel
    {
        public int FlightId { get; set; }

        public string? Seat {  get; set; }

        [Required(ErrorMessage = "Please select a Title!")]
        public string? Title { get; set; }

        [Display(Name = "Full Name")]
        [Required]
        public string? FullName { get; set; }

        [Display(Name = "Identification Number")]
        [Required]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Invalid Identification Number!")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Identification Number consists of only numbers!")]
        public string? IdNumber { get; set; }

        [Display(Name = "Phone Number")]
        [Required]
        [Phone]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Invalid Phone Number!")]
        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}
