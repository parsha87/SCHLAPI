using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Scheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ControllerBase
    {


        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }

    }
}
