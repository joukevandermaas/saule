using System.Net;
using System.Web.Http;

namespace Saule.AspNet.Tests.Controllers
{
    [RoutePrefix("static")]
    public class StaticController : ApiController
    {
        [HttpGet]
        [Route("text")]
        public IHttpActionResult GetStaticFile()
        {
            return this.Content(HttpStatusCode.OK, "This is static content.");
        }
    }
}
