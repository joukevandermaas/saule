using System.Web.Http;
using Tests.Helpers;
using Tests.Models;

namespace Tests.Controllers
{
    public class BrokenController : ApiController
    {
        [HttpGet]
        [Route("api/broken/{id}")]
        public Person GetPerson(string id)
        {
            return Get.Person(id);
        }
    }
}
