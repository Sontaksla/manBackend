using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace manBackend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ClassroomController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateRoom() 
        {


            return Ok();
        }
    }
}
