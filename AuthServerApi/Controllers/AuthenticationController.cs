using AuthServerApi.Model;
using AuthServerApi.Service.UserRepositories;
using AuthServerApi.Service.Utility;
using Microsoft.AspNetCore.Mvc;
using AuthServerApi.Model.Responses;
using AuthServerApi.Model.Requests;
using AuthServerApi.Service.TokenValidator;
using AuthServerApi.Service.RefreshTokenRepositories;
using AuthServerApi.Service.Authenticators;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Google.Apis.Auth.AspNetCore3;
using AuthServerApi.Service;
using Microsoft.AspNetCore.Authentication.Google;

namespace AuthServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly Authenticator _authenticator;
        LinkGenerator _linkGenerator;
        IAccountService _accountService;
        SignInManager<User> _signInManager;
        public AuthenticationController(
            IUserRepository userRepository,
            RefreshTokenValidator refreshTokenValidator,
            IRefreshTokenRepository refreshTokenRepository,
            Authenticator authenticator,
            LinkGenerator linkGenerator,
            IAccountService accountService
            )
        {
            _userRepository = userRepository;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
            _authenticator = authenticator;
            _linkGenerator = linkGenerator;
            _accountService = accountService;
         }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request == null) { 
            return BadRequest();
            }
            if(request.password != request.confPassword)
            {
                return BadRequest(new ErrorResponse("Confirm Password do not match"));
            }
            var existingUser = await _userRepository.GetByEmail(request.email);
            if (existingUser != null) { 
                return Conflict(new ErrorResponse("Email Already Exisit"));
            }

            User user = new User()
            {
                Id = Guid.NewGuid(),
                Email = request.email,
                Name = request.name,
                MobileNumber = request.mobileNumber,
                HashPassword =  Common.PasswordHasher(request.password),
                Dob = request.dob
            };

            var result = await _userRepository.RegisterUser(user);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login ([FromBody] LoginRequest loginRequest)
        {
            var user = await _userRepository.GetByEmail(loginRequest.Email);
            if (user == null)
            {
                return  Unauthorized (new ErrorResponse("Wrong Email or Password"));
            }
            if (user.HashPassword != Common.PasswordHasher(loginRequest.Password)) {

                return Unauthorized(new ErrorResponse("Wrong Email or Password"));
            }

            var authenticatedUserResponse = await _authenticator.Authenticate(user);
            _authenticator.SetTokenInsideCookie(authenticatedUserResponse.RefreshToken, HttpContext);

            return Ok(authenticatedUserResponse);

        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody]RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isValidRefreshToken = _refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken)
            {
                return BadRequest(new ErrorResponse("Invalid Refresh Token"));
            }
            RefreshToken refreshTokenDTO = await _refreshTokenRepository.GetByTokenAsync(refreshRequest.RefreshToken);
            if (refreshTokenDTO == null) {
                return NotFound(new ErrorResponse("Invalid Refresh Token"));
            }
            await _refreshTokenRepository.Delete(refreshTokenDTO.Id);

            var user = await _userRepository.GetById(refreshTokenDTO.UserId);
            if (user == null) {
                return NotFound(new ErrorResponse("User Not Found"));
            }
            var authenticatedUserResponse = await _authenticator.Authenticate(user);
            return Ok(authenticatedUserResponse);
        }

        [HttpGet("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

            if (string.IsNullOrEmpty(refreshToken)) {
                return BadRequest("Token not found");
            }
            bool isValidRefreshToken = _refreshTokenValidator.Validate(refreshToken);
            if (!isValidRefreshToken)
            {
                return BadRequest(new ErrorResponse("Invalid Refresh Token"));
            }
            RefreshToken refreshTokenDTO = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (refreshTokenDTO == null)
            {
                return NotFound(new ErrorResponse("Invalid Refresh Token"));
            }
            await _refreshTokenRepository.Delete(refreshTokenDTO.Id);

            var user = await _userRepository.GetById(refreshTokenDTO.UserId);
            if (user == null)
            {
                return NotFound(new ErrorResponse("User Not Found"));
            }
            var authenticatedUserResponse = await _authenticator.Authenticate(user);
            _authenticator.SetTokenInsideCookie(authenticatedUserResponse.RefreshToken, HttpContext);
            return Ok(authenticatedUserResponse);
        }

        [Authorize]
        [HttpDelete("Logout")]
        public async Task<IActionResult> LogOut()
        {
            string rawUserId = HttpContext.User.FindFirstValue("id");
            if(!Guid.TryParse(rawUserId, out Guid userId))
            {
                return Unauthorized(new ErrorResponse("User Not found"));
            }
            await _refreshTokenRepository.DeleteAll(userId);
            HttpContext.Response.Cookies.Delete("refreshToken");
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            
            return Ok();
        }

        [HttpGet("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromQuery] string returnURL)
        {
           // var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google",
                //_linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback") + $"?returnUrl={returnURL}");

             var properties = new AuthenticationProperties { 
                 RedirectUri = _linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback") + $"?returnUrl={returnURL}" 
             };
            properties.Items["LoginProvider"] = "Google";

            //return properties;

            return Challenge(properties, ["Google"]);
           
        }

        [HttpGet]
        [EndpointName("GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallback([FromQuery] string returnURL)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) {
                return Unauthorized();
            }

            var user =  await _accountService.LoginWithGoogleAsync(result.Principal);
            var authenticatedUserResponse = await _authenticator.Authenticate(user);
            _authenticator.SetTokenInsideCookie(authenticatedUserResponse.RefreshToken, HttpContext);
            var resultURl = $"{returnURL}?accessToken={authenticatedUserResponse.AuthToken}";

            
            return Redirect(resultURl);
        }
    }
}
