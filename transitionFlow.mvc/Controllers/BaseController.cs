using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using TransitFlow.mvc.Models;
using TransitFlow.mvc.Models.DTO;

namespace TransitFlow.mvc.Controllers
{
    public abstract class BaseController : Controller
    {
        private static readonly JsonSerializerOptions ApiJsonOptions = new(JsonSerializerDefaults.Web);

        protected HomeUserModel? GetUser()
        {
            var accessToken = Request.Cookies["accessToken"];
            if (string.IsNullOrEmpty(accessToken)) return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(accessToken)) return null;

                var jwtToken = handler.ReadJwtToken(accessToken);
                var userIdClaim = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int parsedId))
                {
                    return null;
                }

                var roleClaims = jwtToken.Claims
                    .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                var userRoles = new List<AppRole>();
                foreach (var roleStr in roleClaims)
                {
                    if (Enum.TryParse<AppRole>(roleStr, true, out var parsedRole))
                    {
                        userRoles.Add(parsedRole);
                    }
                }

                return new HomeUserModel
                {
                    Id = parsedId,
                    Roles = userRoles
                };
            }
            catch
            {
                return null;
            }
        }

        protected async Task<IActionResult> ForwardApiErrorAsync(HttpResponseMessage response)
        {
            ApiErrorResponseDto? errorResponse = null;

            try
            {
                errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(ApiJsonOptions);
            }
            catch
            {
                errorResponse = null;
            }

            errorResponse ??= new ApiErrorResponseDto();

            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => Unauthorized(errorResponse),
                HttpStatusCode.Forbidden => StatusCode(StatusCodes.Status403Forbidden, errorResponse),
                HttpStatusCode.NotFound => NotFound(errorResponse),
                HttpStatusCode.Conflict => Conflict(errorResponse),
                HttpStatusCode.BadRequest => BadRequest(errorResponse),
                _ => StatusCode((int)response.StatusCode, errorResponse)
            };
        }
    }
}