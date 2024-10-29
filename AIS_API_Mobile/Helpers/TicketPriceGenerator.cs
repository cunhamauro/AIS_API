using AIS_API_Mobile.Data.Entities;

namespace AIS_API_Mobile.Helpers
{
    static class TicketPriceGenerator
    {
        /// <summary>
        /// Generate a price for the flight tickets
        /// </summary>
        /// <returns>Ticker price</returns>
        static public decimal GetTicketPrice(TimeSpan duration, int availableSeats, int aircraftCapacity)
        {
            // Algorithm to generate a price based on the duration of the flight and seat rarity (Not applicable for real use, but for illustrating distinct pricing scenarios)
            int totalMinutes = (int)duration.TotalMinutes;
            decimal basePrice = 25 + (totalMinutes / 10);
            decimal availableSeatsPercent = (decimal)availableSeats / aircraftCapacity;

            decimal price = basePrice + (basePrice * (1 - availableSeatsPercent)) * 3;

            return price;
        }
    }
}
