using System.ComponentModel.DataAnnotations;

namespace AIS_API_Mobile.Data.Entities
{
    public class FlightRecord : IEntity
    {
        public int Id { get; set; } // Flight ID

        [Display(Name = "Flight Number")]
        public string? FlightNumber { get; set; }

        public string? OriginCity { get; set; }

        public string? OriginCountry { get; set; }

        public string? OriginFlagImageUrl { get; set; }

        public string? DestinationCity { get; set; }

        public string? DestinationCountry { get; set; }

        public string? DestinationFlagImageUrl { get; set; }

        public DateTime Departure { get; set; }

        public DateTime Arrival { get; set; }

        public bool Canceled { get; set; }
    }
}
