using Microsoft.EntityFrameworkCore;
using transitFlow.api.Data;
using StopEntity = transitFlow.api.Models.Stop;

namespace transitFlow.api.Repositories
{
    public class StopRepository : IStopRepository
    {
        private readonly TransitFlowDbContext _context;

        public StopRepository(TransitFlowDbContext context)
        {
            _context = context;
        }

        public IQueryable<StopEntity> GetQueryable()
        {
            return _context.Stops.AsNoTracking(); 
        }

        public async Task<IEnumerable<StopEntity>> GetAllAsync()
        {
            return await _context.Stops.AsNoTracking().ToListAsync();
        }

        public async Task<StopEntity?> GetByIdAsync(int id)
        {
            return await _context.Stops.FindAsync(id);  
        }

        public async Task<StopEntity> CreateAsync(StopEntity stop)
        {
            _context.Stops.Add(stop);
            await _context.SaveChangesAsync();
            return stop;
        }

        public async Task<bool> DeleteAsync(StopEntity stop)
        {
            _context.Stops.Remove(stop);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}