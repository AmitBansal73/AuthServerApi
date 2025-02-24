using AuthServerApi.Data;
using AuthServerApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthServerApi.Service.RefreshTokenRepositories
{
    public class SqlRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthenticationDbContext _context;
        public SqlRefreshTokenRepository(AuthenticationDbContext context) {
        _context = context;
        }
        public async Task Create(RefreshToken refreshToken)
        {
            _context.refreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid refreshTokenId)
        {
            RefreshToken refreshToken = await _context.refreshTokens.FindAsync(refreshTokenId);
            if (refreshToken != null) {
                _context.refreshTokens.Remove(refreshToken);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll(Guid UserId)
        {
            IEnumerable<RefreshToken> refreshTokens = await _context.refreshTokens.Where(x => x.UserId == UserId)
                                                            .ToListAsync<RefreshToken>();
            _context.refreshTokens.RemoveRange(refreshTokens);
            _context.SaveChanges();
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
          return  await _context.refreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        }
    }
}
