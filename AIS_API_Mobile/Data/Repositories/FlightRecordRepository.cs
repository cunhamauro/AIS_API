using AIS_API_Mobile.Data.Entities;

namespace AIS_API_Mobile.Data.Repositories
{
    public class FlightRecordRepository : GenericRepository<FlightRecord>, IFlightRecordRepository
    {
        public FlightRecordRepository(DataContext context) : base(context)
        {
        }
    }
}
