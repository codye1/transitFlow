using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace transitFlow.api.Models
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Stop> CreatedStops { get; set; } = new List<Stop>();
        public virtual ICollection<Vehicle> CreatedVehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<Route> CreatedRoutes { get; set; } = new List<Route>();
    }
}
