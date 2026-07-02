using RouteEntity = transitFlow.api.Models.Route;

namespace transitFlow.api.Repositories.Route
{
    public interface IRouteRepository
    {
        Task<IEnumerable<RouteEntity>> GetAllRoutesAsync();
        Task<RouteEntity?> GetRouteByIdAsync(int id);
        Task<bool> RouteNumberExistsAsync(string number);
        Task<RouteEntity> CreateRouteAsync(RouteEntity route);
        Task<bool> DeleteRouteAsync(int id);
        Task<IEnumerable<RouteEntity>> GetRoutesCursorAsync(int? afterId, int take);
    }
}
