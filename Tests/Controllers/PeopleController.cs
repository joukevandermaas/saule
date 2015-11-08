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
        public Person GetPerson(string id)
        {
            return new Person(prefill: true, id: id);
        }

        [HttpGet]
        public IEnumerable<Person> GetPeople()
        {
            return GetPersonEnumerable().Take(100);
        }

        [HttpPost]
        public Person PostPerson(string id, Person person)
        {
            return person;
        }

        private static IEnumerable<Person> GetPersonEnumerable()
        {
            var i = 0;
            while (true)
            {
                yield return new Person(prefill: true, id: i++.ToString());
            }
        }
    }
}