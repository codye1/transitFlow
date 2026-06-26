namespace transitFlow.api.Models.DTO.Stop
{
    public class StopResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
