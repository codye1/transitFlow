namespace TransitFlow.mvc.Models
{
    public class VehicleItemViewModel
    {
        public VehicleModel Vehicle { get; set; } = default!;
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