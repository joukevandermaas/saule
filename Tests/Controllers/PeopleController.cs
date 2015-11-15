using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;

namespace Tests.Controllers
{
    [ReturnsResource(typeof(PersonResource))]
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
        [Route("people/query")]
        public IEnumerable<Person> QueryPeople()
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
            return Get.People(100);
        }
    }
}