using Microsoft.AspNetCore.Mvc;

namespace HumanCentricAttackPath.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttackController : ControllerBase
    {
        [HttpGet("top_paths")]
        public IActionResult GetTopPaths()
        {
            // Your logic here
            return Ok(new { message = "Top paths endpoint works!" });
        }
    }
}