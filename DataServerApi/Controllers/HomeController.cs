using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DataServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new {Message = "Method is Accessible"});
        }

        [Authorize]
        [HttpGet("Home")]
        public async Task<IActionResult> Home()
        {
            string id = HttpContext.User.FindFirstValue("id");
            string name = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            string email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return Ok(new { id, name, email });
        }
    }
}
