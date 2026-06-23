namespace transitFlow.api.Models.DTO.Route
{
    public class CreateRouteDto
    {
        public string Number { get; set; } 
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public List<int> SelectedStops { get; set; } = new List<int>();
    }
}
