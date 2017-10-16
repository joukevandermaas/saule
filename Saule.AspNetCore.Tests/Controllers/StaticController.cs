using Microsoft.AspNetCore.Mvc;

namespace Saule.AspNetCore.Tests.Controllers
{
    [Route("static")]
    public class StaticController : Controller
    {
        [HttpGet]
        [Route("text")]
        public IActionResult GetStaticFile()
        {
            return this.Content("This is static content.");
        }
    }
}
