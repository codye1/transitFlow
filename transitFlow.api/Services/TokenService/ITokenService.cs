using transitFlow.api.Models;

namespace transitFlow.api.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user, IList<string> roles);
        string GenerateRefreshToken();
        DateTime AccessTokenExpiry { get; }
    }
}