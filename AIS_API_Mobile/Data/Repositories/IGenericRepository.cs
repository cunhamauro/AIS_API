using System.Linq;
using System.Threading.Tasks;

namespace AIS_API_Mobile.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task CreateAsync(T entity);

        Task<bool> ExistAsync(int id);

        Task<T> GetByIdAsync(int id);

        Task<T> GetByIdTrackAsync(int id);

        IQueryable<T> GetAll();

        Task DeleteAsync(T entity);

        Task UpdateAsync(T entity);
    }
}
