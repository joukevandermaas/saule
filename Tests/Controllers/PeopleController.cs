using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;

namespace Tests.Controllers
{
    [ReturnsResource(typeof(PersonResource))]
    [RoutePrefix("api")]
    public class PeopleController : ApiController
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
        public Person PostPerson(string id, Person person)
        {
            person.Identifier = id;
            return person;
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