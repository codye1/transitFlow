namespace TransitFlow.mvc.Models
{
    public class StopSidebarViewModel
    {
        public IEnumerable<StopModel> Stops { get; set; }
        public HomeUserModel User { get; set; }
    }
}
