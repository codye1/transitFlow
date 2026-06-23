using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace transitFlow.api.Models
{
    public enum VehicleType
    {
        Bus,
        Minibus,
        Trolleybus,
        Tram,
    }

    public enum VehicleStatus
    {
        OnRoute,
        AtDepot,
        Maintenance
    }
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PlateNumber { get; set; }

        [Required]
        public VehicleType Type { get; set; }

        [Required]
        public string Model { get; set; }

        public int? RouteId { get; set; }
        public int Capacity { get; set; }
        public VehicleStatus Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public int CreatedById { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CreatedById))]
        public virtual AppUser Creator { get; set; }
    }
}
