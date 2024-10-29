using AIS_API_Mobile.Data.Entities;
using AIS_API_Mobile.Helpers;

namespace AIS_API_Mobile.Data.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(DataContext context) : base(context)
        {
        }
    }
}
