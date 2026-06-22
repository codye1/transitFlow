using transitFlow.api.Models;
using StopEntity = transitFlow.api.Models.Stop;

namespace transitFlow.api.Repositories
{
    public interface IStopRepository
    {
        IQueryable<StopEntity> GetQueryable();
        Task<IEnumerable<StopEntity>> GetAllAsync();
        Task<StopEntity?> GetByIdAsync(int id);
        Task<StopEntity> CreateAsync(StopEntity stop);
        Task<bool> DeleteAsync(StopEntity stop);
    }
}