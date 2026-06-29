namespace TransitFlow.mvc.Models
{

    public class VehicleSidebarViewModel
    {
        public enum VehicleStatus
        {
            OnRoute,     // На маршруті
            AtDepot,     // У депо
            Maintenance  // Обслуговування
        }
        public List<VehicleModel> Vehicles { get; set; } = new();
        public List<RouteModel> Routes { get; set; } = new();
        public HomeUserModel? User { get; set; }
        public Dictionary<string, string> AvailableStatuses { get; set; } = new()
        {
            { "AtDepot", "У депо" },
            { "OnRoute", "На маршруті" },
            { "Maintenance", "Обслуговування" }
        };
    }
}
