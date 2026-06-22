using System.ComponentModel.DataAnnotations;

namespace transitFlow.api.Models.DTO.Stop
{
    public class StopCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }
}
