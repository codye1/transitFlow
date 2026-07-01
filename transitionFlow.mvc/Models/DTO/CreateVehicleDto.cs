namespace TransitFlow.mvc.Models.DTO
{
    public class CreateVehicleDto
    {
        public string PlateNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? RouteId { get; set; }
        public int Capacity { get; set; }
    }
}