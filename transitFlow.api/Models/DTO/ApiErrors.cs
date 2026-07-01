using Microsoft.AspNetCore.Identity;

namespace transitFlow.api.Models.DTO
{
    public static class ApiErrors
    {
        public static List<ApiErrorDto> Single(string code, string message)
        {
            return new List<ApiErrorDto>
            {
                new ApiErrorDto(code, message)
            };
        }

        public static List<ApiErrorDto> FromIdentityErrors(IEnumerable<IdentityError> errors)
        {
            return errors.Select(error => new ApiErrorDto(error.Code, error.Description)).ToList();
        }
    }
}