using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Saule.Common.Tests.Helpers;
using Saule.Common.Tests.Models;
using Saule.Http;
using Saule.Queries;

namespace Saule.AspNetCore.Tests.Controllers
{
    public class BrokenController : Controller
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
        //[Authorize]
        public IQueryable<Person> GetPeople()
        {
            return Get.People(1).ToList()[2].ToEnumerable().AsQueryable();
        }
    }
}
