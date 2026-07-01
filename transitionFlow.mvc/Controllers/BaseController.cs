using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TransitFlow.mvc.Models;

namespace TransitFlow.mvc.Controllers
{
    public abstract class BaseController : Controller
    {
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
    }
}