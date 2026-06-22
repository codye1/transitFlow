using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using transitFlow.api.Models;

public class Route
{
    [Key]
    public int Id { get; set; }

    public int Number { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Color { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public int CreatedById { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CreatedById))]
    public virtual AppUser Creator { get; set; }

    public virtual ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
}