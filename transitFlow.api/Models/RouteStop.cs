using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using transitFlow.api.Models;

public class RouteStop
{
    [Key]
    public int Id { get; set; }

    public int RouteId { get; set; }

    public int StopId { get; set; }

    [ForeignKey(nameof(RouteId))]
    public virtual Route Route { get; set; }

    [ForeignKey(nameof(StopId))]
    public virtual Stop Stop { get; set; }
}