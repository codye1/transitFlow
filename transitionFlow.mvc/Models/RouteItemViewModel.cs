namespace TransitFlow.mvc.Models
{
    public class RouteItemViewModel
    {
        public RouteModel Route { get; set; } = default!;
        public HomeUserModel? User { get; set; }
    }
}
