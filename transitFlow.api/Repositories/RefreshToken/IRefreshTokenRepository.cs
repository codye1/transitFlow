using System.Threading.Tasks;
using transitFlow.api.Models;

namespace transitFlow.api.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        void Add(RefreshToken refreshToken);
        void Update(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}