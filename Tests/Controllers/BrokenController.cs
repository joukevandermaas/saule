using System.Linq;
using System.Web.Http;
using Saule.Http;
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

        [HttpGet]
        [ReturnsResource(typeof(PersonResource))]
        [Route("api/broken")]
        [Authorize]
        public IQueryable<Person> GetPeople()
        {
            return Get.People(20).AsQueryable();
        }
    }
}
