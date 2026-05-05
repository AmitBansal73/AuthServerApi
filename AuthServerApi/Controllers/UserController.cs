using AuthServerApi.Model.Requests;
using AuthServerApi.Service.UserRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet("Users")]
        public async Task<IActionResult> Users()
        {
            var result = await _userRepository.GetAll();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("UpdatePersonalInfo")]
        public async Task<IActionResult> UpdatePersonalInfo([FromBody] UserInfoRequest request)
        {
            var result = await _userRepository.UpdatePersonalInfo(request);
            if (result)
                return Ok(new { message = "Personal information updated successfully" });
            else
                return BadRequest(new { message = "Failed to update personal information" });


        }
    }
}
