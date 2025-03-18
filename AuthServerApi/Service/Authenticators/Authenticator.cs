using AuthServerApi.Model;
using AuthServerApi.Model.Responses;
using AuthServerApi.Service.RefreshTokenRepositories;
using AuthServerApi.Service.TokenGenerator;
using AuthServerApi.Service.UserRepositories;

namespace AuthServerApi.Service.Authenticators
{
    public class Authenticator
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AccessTokenGenerator _tokenGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;

        public Authenticator(
            IRefreshTokenRepository refreshTokenRepository, 
            AccessTokenGenerator tokenGenerator, 
            RefreshTokenGenerator refreshTokenGenerator)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenGenerator = tokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
        }
        public async Task<AuthenticatedUserResponse> Authenticate(User user)
        {
            string accessToken = _tokenGenerator.GenerateToken(user);
            string refreshToken = _refreshTokenGenerator.GenerateToken();
            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };
            await _refreshTokenRepository.Create(refreshTokenDTO);

            return new AuthenticatedUserResponse()
            {
                AuthToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public void SetTokenInsideCookie(string refreshToken, HttpContext context)
        {
            context.Response.Cookies.Append("refreshToken", refreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

        }

        public void ClearCookie(HttpContext context) {
            context.Response.Cookies.Delete("refreshToken");
        }
    }
}
