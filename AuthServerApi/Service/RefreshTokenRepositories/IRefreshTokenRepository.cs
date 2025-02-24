using AuthServerApi.Model;

namespace AuthServerApi.Service.RefreshTokenRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);   
        Task Create(RefreshToken refreshToken);
        Task Delete(Guid refreshTokenId);

        Task DeleteAll(Guid UserId);
    }
}
