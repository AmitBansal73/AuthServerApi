using AuthServerApi.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthServerApi.Service.TokenGenerator
{
    public class AccessTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;

        public AccessTokenGenerator(IOptions<AuthenticationConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }
        public string GenerateToken(User user)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.AccessTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.Name.ToString()),
               new Claim(ClaimTypes.Email, user.Email),
            };
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration.Issuer,
                _configuration.Audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(_configuration.AccessTokenExpirationMinutes),
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
