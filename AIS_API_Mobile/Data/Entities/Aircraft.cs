using AIS_API_Mobile.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Data.Entities
{
    public class Aircraft : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Aircraft model is required!")]
        public string? Model { get; set; }

        [Required]
        [Range(10, 260, ErrorMessage = "Aircraft capacity must be between {1} and {2}!")]
        //[CapacityDivisibleByRows("Rows")]
        public int Capacity { get; set; }

        public List<string> Seats { get; set; } = new List<string>();

        [Required]
        [Range(2, 26, ErrorMessage = "Aircraft rows must be between {1} and {2}!")]
        public int Rows {  get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public User? User { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        //[Display(Name = "Image")]
        //public string ImageDisplay
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(ImageUrl))
        //        {
        //            return $"/images/noimage.png";
        //        }
        //        else
        //        {
        //            return $"{ImageUrl.Substring(1)}";
        //        }
        //    }
        //}
    }
}
