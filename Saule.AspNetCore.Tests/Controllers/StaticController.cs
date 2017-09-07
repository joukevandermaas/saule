namespace Tests.Controllers
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

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
