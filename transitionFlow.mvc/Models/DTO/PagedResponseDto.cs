namespace TransitFlow.mvc.Models.DTO
{

    public class PagedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();
        public bool HasMore { get; set; }
    }
}
