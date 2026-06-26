namespace transitFlow.api.Models.DTO.Route
{
    public class RouteResponseDto
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public int CreatedById { get; set; } 
        public List<int> Stops { get; set; } = new List<int>();
    }
}
