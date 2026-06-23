namespace transitFlow.api.Models.DTO.Vehicle
{
    public class VehicleResponseDto
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public VehicleType Type { get; set; }
        public VehicleStatus Status { get; set; }
        public string Model { get; set; }
        public int? RouteId { get; set; }
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
    }
}