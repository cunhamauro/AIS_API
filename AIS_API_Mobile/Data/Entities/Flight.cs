using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace AIS_API_Mobile.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }

        public Aircraft? Aircraft {  get; set; }

        [Display(Name = "Available Seats")]
        public List<string>? AvailableSeats { get; set; }

        public Airport? Origin { get; set; }

        public Airport? Destination { get; set; }

        [Required]
        public DateTime Departure { get; set; }

        [Required]
        public DateTime Arrival { get; set; }

        public TimeSpan Duration => Arrival - Departure;

        [Display(Name = "Flight")]
        public string? FlightNumber { get; set; }

        public User? User { get; set; }

        public List<Ticket> TicketList { get; set; } = new List<Ticket>();

        #region Methods

        /// <summary>
        /// Update the available seats of the flight
        /// </summary>
        /// <param name="seat">Seat</param>
        /// <param name="removeSeat">Seat to be removed?</param>
        public void UpdateAvailableSeats(string seat, bool removeSeat)
        {
            if (removeSeat)
            {
                AvailableSeats.Remove(seat);
            }
            else
            {
                AvailableSeats.Add(seat);
            }

            AvailableSeats.OrderBy(s => s);
        }

        /// <summary>
        /// Update the ticket list of the flight
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <param name="removeTicket">Ticket to be removed?</param>
        public void UpdateTicketList(Ticket ticket, bool removeTicket)
        {
            string seat = ticket.Seat;

            if (removeTicket)
            {
                UpdateAvailableSeats(seat, false);
                TicketList.Remove(ticket);
            }
            else
            {
                TicketList.Add(ticket);
                UpdateAvailableSeats(seat, true);
            }
        }

        #endregion
    }
}
