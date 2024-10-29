using AIS_API_Mobile.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIS_API_Mobile.Data.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all Flights including nested Entities
        /// </summary>
        /// <returns>List of Flights</returns>
        public async Task<List<Flight>> GetFlightsIncludeAsync()
        {
            return await _context.Flights
            .Include(f => f.Aircraft)
            .Include(f => f.Origin)
            .Include(f => f.Destination).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Get a Flight by ID including nested Entities
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>Flight</returns>
        public async Task<Flight> GetFlightIncludeByIdAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .Include(f => f.TicketList)
                .AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Get a Flight by ID including nested Entities tracked
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>Flight</returns>
        public async Task<Flight> GetFlightIncludeByIdTrackAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .Include(f => f.TicketList)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
