namespace TransitFlow.mvc.Models.DTO
{
    public class CreateRouteDto
    {
        public string Number { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public List<int> SelectedStops { get; set; } = new();
    }
}