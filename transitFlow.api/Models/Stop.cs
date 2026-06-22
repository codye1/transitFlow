using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace transitFlow.api.Models
{
    public class Stop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int CreatedById { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public virtual AppUser Creator { get; set; }

        public virtual ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
    }
}
