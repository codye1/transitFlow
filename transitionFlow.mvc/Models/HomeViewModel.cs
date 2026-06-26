namespace TransitFlow.mvc.Models
{
    public class HomeViewModel
    {
        public List<RouteModel> Routes { get; set; } = new();
        public List<StopModel> Stops { get; set; } = new();
        public List<VehicleModel> Vehicles { get; set; } = new();
    }

    public class RouteModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Status { get; set; }
        public int[] Stops { get; set; }
    }

    public class StopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class VehicleModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string RouteId { get; set; }
    }
}