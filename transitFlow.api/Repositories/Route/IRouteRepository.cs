using RouteEntity = transitFlow.api.Models.Route;

namespace transitFlow.api.Repositories.Route
{
    public interface IRouteRepository
    {
        Task<IEnumerable<RouteEntity>> GetAllRoutesAsync();
        Task<RouteEntity?> GetRouteByIdAsync(int id);
        Task<RouteEntity> CreateRouteAsync(RouteEntity route);
        Task<bool> DeleteRouteAsync(int id);
    }   
}
