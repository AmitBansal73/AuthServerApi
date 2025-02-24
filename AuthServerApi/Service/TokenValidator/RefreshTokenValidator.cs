using AuthServerApi.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthServerApi.Service.TokenValidator
{
    public class RefreshTokenValidator
    {
        AuthenticationConfiguration _configuration;
        public RefreshTokenValidator(IOptions<AuthenticationConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public bool Validate(string refreshToken)
        {
            TokenValidationParameters _validationParameters = null;
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            _validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.RefreshTokenSecret)),
                ValidIssuer = _configuration.Issuer,
                ValidAudience = _configuration.Audience,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true
            };
            try
            {
                tokenHandler.ValidateToken(refreshToken, _validationParameters, out SecurityToken validatedToken);
            }
            catch (Exception ex) { 
            return false;
            }


            return true;
        }
    }
}
