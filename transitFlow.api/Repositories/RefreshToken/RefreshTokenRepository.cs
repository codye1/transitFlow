using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using transitFlow.api.Data;
using transitFlow.api.Models;

namespace transitFlow.api.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TransitFlowDbContext _context;

        public RefreshTokenRepository(TransitFlowDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}