using System.ComponentModel.DataAnnotations;

namespace transitFlow.api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Stop> CreatedStops { get; set; } = new List<Stop>();
        public virtual ICollection<Vehicle> CreatedVehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<Route> CreatedRoutes { get; set; } = new List<Route>();
    }
}
