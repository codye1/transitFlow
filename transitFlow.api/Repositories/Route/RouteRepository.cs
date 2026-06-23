using Microsoft.EntityFrameworkCore;
using transitFlow.api.Data;
using transitFlow.api.Models;
using transitFlow.api.Repositories.Route;
using RouteEntity = transitFlow.api.Models.Route; 
namespace transitFlow.api.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly TransitFlowDbContext _context;

        public RouteRepository(TransitFlowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RouteEntity>> GetAllRoutesAsync()
        {
            return await _context.Routes
                .AsNoTracking()
                .Include(r => r.RouteStops)
                .ToListAsync();
        }

        public async Task<RouteEntity?> GetRouteByIdAsync(int id)
        {
            return await _context.Routes
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RouteEntity> CreateRouteAsync(RouteEntity route)
        {
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
            return route;
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null) return false;

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RouteEntity>> GetRoutesCursorAsync(int? afterId, int take)
        {
            IQueryable<RouteEntity> query = _context.Routes
                .AsNoTracking()
                .Include(r => r.RouteStops);

            if (afterId.HasValue && afterId.Value > 0)
            {
                query = query.Where(r => r.Id > afterId.Value);
            }

            return await query
                .OrderBy(r => r.Id)
                .Take(take + 1)
                .ToListAsync();
        }
    }
}