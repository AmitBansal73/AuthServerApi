using AuthServerApi.Model;
using AuthServerApi.Model.Requests;
using AuthServerApi.Model.Responses;
using AuthServerApi.Service.Authenticators;
using AuthServerApi.Service.UserRepositories;
using Microsoft.AspNetCore.Mvc;

namespace AuthServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly Authenticator _authenticator;
        protected readonly IConfiguration _configuration;

        public MobileController(IUserRepository userRepository, Authenticator authenticator)
        {
            _userRepository = userRepository;
            _authenticator = authenticator;
        }

        [HttpPost("GetOTP")]
        public async Task<IActionResult> GetOTP([FromBody] OTPRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (String.IsNullOrEmpty(request.MobileNumber) || request.MobileNumber.Length != 10)
            {
                return BadRequest(new ErrorResponse("Invalid Mobile Number"));
            }
            var otp = await _userRepository.CreateOTP(request.MobileNumber);
            if (otp == null)
            {
                return BadRequest(new ErrorResponse("Failed to create OTP"));
            }

            return Ok(otp);
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (String.IsNullOrEmpty(request.OTP) || request.OTP.Length != 4)
            {
                return BadRequest(new ErrorResponse("OTP"));
            }
            var otp = await _userRepository.VerifyOTP(request);

            if (!otp)
            {
                return BadRequest(new ErrorResponse("Failed to Verify OTP"));
            }

            var user = await _userRepository.GetByMobile(request.MobileNumber);
            if (user == null)
            {
                user = new User()
                {
                    Id = Guid.NewGuid(),
                    Email = String.Empty,
                    Name = String.Empty,
                    MobileNumber = request.MobileNumber,
                    HashPassword = String.Empty,
                    Dob = DateOnly.FromDateTime(DateTime.MinValue)
                };

                var result = await _userRepository.RegisterUser(user);
            }

            var authenticatedUserResponse = await _authenticator.Authenticate(user);
            _authenticator.SetTokenInsideCookie(authenticatedUserResponse.RefreshToken, HttpContext);

            return Ok(authenticatedUserResponse);

        }
    }
}
