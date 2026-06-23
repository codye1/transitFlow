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

        public async Task<bool> AreStopsValidAsync(List<int> stopIds)
        {
            if (stopIds == null || !stopIds.Any()) return true;
            var uniqueIds = stopIds.Distinct().ToList();
            var count = await _context.Stops.AsNoTracking().Where(s => uniqueIds.Contains(s.Id)).CountAsync();
            return count == uniqueIds.Count;
        }

        public async Task<IEnumerable<StopEntity>> GetStopsCursorAsync(int? afterId, int take, int? routeId = null)
        {
            var query = _context.Stops.AsNoTracking();

            if (routeId.HasValue)
            {
                query = query.Where(s => s.RouteStops.Any(rs => rs.RouteId == routeId.Value));
            }
                                                                                                        
            if (afterId.HasValue && afterId.Value > 0)
            {
                query = query.Where(s => s.Id > afterId.Value);
            }

            return await query
                .OrderBy(s => s.Id)
                .Take(take + 1)
                .ToListAsync();
        }
    }
}