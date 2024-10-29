using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Profile Image")]
        public string? ImageUrl { get; set; }

        //[Display(Name = "Profile Image")]
        //public string ImageDisplay
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(ImageUrl))
        //        {
        //            return $"/images/default-profile-image.png";
        //        }
        //        else
        //        {
        //            return $"{ImageUrl.Substring(1)}";
        //        }
        //    }
        //}
    }
}
