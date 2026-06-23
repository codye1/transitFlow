namespace transitFlow.api.Models.DTO.Vehicle
{
    public class PatchVehicleDto
    {
        public int? RouteId { get; set; }

        public VehicleStatus? Status { get; set; }
    }
}   