namespace TransitFlow.mvc.Models
{

    public class VehicleSidebarViewModel
    {
        public List<VehicleModel> Vehicles { get; set; } = new();
        public List<RouteModel> Routes { get; set; } = new();
        public HomeUserModel? User { get; set; }
        public Dictionary<string, string> AvailableStatuses { get; set; } = new()
        {
            { "at_depot", "У депо" },
            { "en_route", "На маршруті" },
            { "maintenance", "Обслуговування" },
            { "broken", "Поламаний" }
        };
    }
}
