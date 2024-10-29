using AIS_API_Mobile.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AIS_API_Mobile.Data.Repositories
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        Task<List<Flight>> GetFlightsIncludeAsync();

        Task<Flight> GetFlightIncludeByIdAsync(int id);

        Task<Flight> GetFlightIncludeByIdTrackAsync(int id);
    }
}
