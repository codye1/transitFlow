using System.Collections.Generic;

namespace TransitFlow.mvc.Models
{
    public class RouteSidebarViewModel
    {
        public IEnumerable<RouteModel> Routes { get; set; } = new List<RouteModel>();
        public IEnumerable<StopModel> AllStops { get; set; } = new List<StopModel>();
        public HomeUserModel? User { get; set; }
    }
}