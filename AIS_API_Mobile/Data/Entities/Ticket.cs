using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Data.Entities
{
    public class Ticket : IEntity
    {
        public int Id {  get; set; }

        [Required(ErrorMessage = "Please select a Seat!")]
        public string? Seat { get; set; }

        public Flight? Flight { get; set; }

        // User that bought the ticket
        public User? User { get; set; }

        // Properties for the ticket holder (Not mapped as an entity)
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

        public decimal Price { get; set; }
    }
}
