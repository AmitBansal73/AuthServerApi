using AuthServerApi.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthServerApi.Service.TokenGenerator
{
    public class RefreshTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;

        public RefreshTokenGenerator(IOptions<AuthenticationConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }
        public string GenerateToken()
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.RefreshTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration.Issuer,
                _configuration.Audience,
                null,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(_configuration.RefreshTokenExpirationDays),
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
