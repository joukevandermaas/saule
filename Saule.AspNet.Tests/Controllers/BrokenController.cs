using System.Linq;
using System.Web.Http;
using Saule.Common.Tests.Helpers;
using Saule.Common.Tests.Models;
using Saule.Http;

namespace Saule.AspNet.Tests.Controllers
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
