using System.Collections.Generic;
using System.Web.Http;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;

namespace Tests.Controllers
{
    [ReturnsResource(typeof(PersonResource))]
    public class PeopleController : ApiController
    {
        public IEnumerable<Person> People { get; } = Get.People(100);

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
            return GetPeople();
        }

        [HttpGet]
        [Paginated]
        [AllowsQuery]
        [Route("paginate/query/people")]
        public IEnumerable<Person> PaginateAndQueryPeople()
        {
            return GetPeople();
        }

        [HttpPost]
        [Route("people/{id}")]
        public Person PostPerson(string id, Person person)
        {
            person.Id = id;
            return person;
        }

        [HttpGet]
        [Route("people")]
        public IEnumerable<Person> GetPeople()
        {
            return People;
        }
    }
}