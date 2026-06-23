
namespace transitFlow.api.Repositories.Vehicle
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Models.Vehicle>> GetVehiclesCursorAsync(int? afterId, int take);
        Task<Models.Vehicle> CreateVehicleAsync(Models.Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(int id);
        Task<Models.Vehicle?> GetVehicleByIdAsync(int id);
        Task<bool> UpdateVehicleAsync(Models.Vehicle vehicle);
        Task<(bool PlateExists, bool RouteExists)> ValidateVehicleCreationAsync(string plateNumber, int? routeId);
    }
}