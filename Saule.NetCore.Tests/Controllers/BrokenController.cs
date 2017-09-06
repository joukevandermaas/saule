using System.Linq;
using Saule.Queries;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers
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
