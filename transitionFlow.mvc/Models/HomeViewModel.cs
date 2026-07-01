namespace TransitFlow.mvc.Models
{
    public enum AppRole
    {
        User = 1,
        Admin = 2,
        Moderator = 3
    }

    public class HomeViewModel
    {
        public List<RouteModel> Routes { get; set; } = new();
        public List<StopModel> Stops { get; set; } = new();
        public List<VehicleModel> Vehicles { get; set; } = new();
        public HomeUserModel? User { get; set; }
    }

    public class RouteModel
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public int[] Stops { get; set; } = Array.Empty<int>();
    }

    public class StopModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CreatedById { get; set; }
    }

    public class VehicleModel
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; 
        public string Model { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int? RouteId { get; set; } 
        public string Status { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public string TypeDescription => Type?.ToLowerInvariant() switch
        {
            "bus" => "Автобус",
            "tram" => "Трамвай",
            "trolleybus" => "Тролейбус",
            _ => "Невідомо"
        };
    }

    public class HomeUserModel
    {
        public int Id { get; set; }
        public List<AppRole> Roles { get; set; } = new();
    }
}