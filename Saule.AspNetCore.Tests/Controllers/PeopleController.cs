using System.Collections.Generic;
using System.Linq;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers
{
    [ReturnsResource(typeof(PersonResource))]
    [Route("api")]
    public class PeopleController : Controller
    {
        [HttpGet]
        [Route("people/{id}")]
        public Person GetPerson(string id)
        {
            return Get.Person(id);
        }

        [HttpGet]
        [AllowsQuery]
        [Route("query/people")]
        public IEnumerable<Person> QueryPeople()
        {
            return GetPeople();
        }

        [HttpGet]
        [AllowsQuery]
        [Paginated]
        [Route("query/paginate/people")]
        public IEnumerable<Person> QueryAndPaginatePeople()
        {
            return GetPeopleNotRandom();
        }

        [HttpGet]
        [Paginated]
        [AllowsQuery]
        [Route("paginate/query/people")]
        public IEnumerable<Person> PaginateAndQueryPeople()
        {
            return GetPeopleNotRandom();
        }

        [HttpPost]
        [Route("people/{id}")]
        public IActionResult PostPerson(string id, [FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            person.Identifier = id;
            return Ok(person);
        }

        [HttpGet]
        [Route("people")]
        public IEnumerable<Person> GetPeople()
        {
            return Get.People(100);
        }

        private static IEnumerable<Person> GetPeopleNotRandom()
        {
            for (var i = 0; i < 100; i++)
            {
                var person = Get.Person(i.ToString());
                person.Age = i + 2;
                yield return person;
            }
        }
    }
}