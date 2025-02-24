using Microsoft.AspNetCore.Mvc;

namespace DataServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        [HttpGet("top3")]
        public async Task<IActionResult> top3()
        {
            return Ok();
        }

        [HttpGet("popular")]
        public async Task<IActionResult> popular() {
            return Ok();
        }
    }
}
