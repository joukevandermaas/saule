namespace Tests.Controllers
{
    using System.Net;
    using System.Web.Http;
   
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
