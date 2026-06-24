namespace transitFlow.api.Models.DTO
{
    public class ApiErrorDto
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ApiErrorDto(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}