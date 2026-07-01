namespace TransitFlow.mvc.Models.DTO
{
    public class StopCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}