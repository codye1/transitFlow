namespace TransitFlow.mvc.Models
{
    public class StopItemViewModel
    {
        public StopModel Stop { get; set; } = default!;
        public HomeUserModel? User { get; set; }
    }
}