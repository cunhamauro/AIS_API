using AIS_API_Mobile.Data.Entities;

namespace AIS_API_Mobile.Data.Repositories
{
    public class TicketRecordRepository : GenericRepository<TicketRecord>, ITicketRecordRepository
    {
        public TicketRecordRepository(DataContext context) : base(context)
        {
        }
    }
}
