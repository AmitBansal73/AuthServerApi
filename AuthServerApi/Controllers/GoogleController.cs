using AuthServerApi.Model;
using AuthServerApi.Model.Requests;
using AuthServerApi.Service;
using AuthServerApi.Service.Authenticators;
using AuthServerApi.Service.UserRepositories;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace AuthServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    { 
        IAccountService _accountService;
        IUserRepository _userRepository;
        private readonly Authenticator _authenticator;
        public GoogleController(IAccountService accountService, IUserRepository userRepository, Authenticator authenticator)
        {
            _accountService = accountService;
            _userRepository = userRepository;
            _authenticator = authenticator;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] GoogleLoginRequest request)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new string[] { "672851075451-klpqls3c6685uv374j86jnajujvgbi0e.apps.googleusercontent.com" }
            };
            
            var result = await GoogleJsonWebSignature.ValidateAsync(request.googleToken,settings);

            if (result == null) {
                return Unauthorized();
            }

            var existingUser = await _userRepository.GetByEmail(result.Email);

            if (existingUser == null) {
                User _googleUser = new User()
                {
                    Email = result.Email,
                    Name = result.Name,
                    MobileNumber = ""
                };

              existingUser = await  _userRepository.RegisterUser(_googleUser);
            }

            var authenticatedUserResponse = await _authenticator.Authenticate(existingUser);
            _authenticator.SetTokenInsideCookie(authenticatedUserResponse.RefreshToken, HttpContext);
            return Ok(authenticatedUserResponse);
        }
    }
}
