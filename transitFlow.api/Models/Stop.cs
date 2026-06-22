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

        public int Latitude { get; set; }

        public int Longitude { get; set; }

        public int CreatedById { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public virtual User Creator { get; set; }

        public virtual ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
    }
}
