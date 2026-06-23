using System.ComponentModel.DataAnnotations;

namespace transitFlow.api.Models.DTO.Route
{
    public class CreateRouteDto
    {
        [Required]
        public string Number { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Color { get; set; }

        [MinLength(2, ErrorMessage = "At least two stops are required")]
        public List<int> SelectedStops { get; set; } = new List<int>();
    }
}
