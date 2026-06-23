using System.ComponentModel.DataAnnotations;

namespace transitFlow.api.Models.DTO.Vehicle
{
    public class CreateVehicleDto
    {
        [Required]
        public string PlateNumber { get; set; }

        [Required]
        public VehicleType Type { get; set; }
        public VehicleStatus Status { get; set; } = VehicleStatus.AtDepot;
        [Required]
        public string Model { get; set; }

        public int? RouteId { get; set; }
        [Required]
        public int Capacity { get; set; }
    }
}