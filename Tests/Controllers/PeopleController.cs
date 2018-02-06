using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Saule.Queries;
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

        [HttpGet]
        [HandlesQuery]
        [Paginated]
        [Route("query/manual/paginate/people")]
        public IEnumerable<Person> ManualQueryAndPaginatePeople(QueryContext context)
        {
            IEnumerable<Person> data = GetPeopleNotRandom(10).ToList();
            
            bool? hideLastName;
            // if we want to include car or job, then we return response as is as it already has them
            // otherwise we clear it
            bool includeCar = context.Include.Includes.Any(p => p.Name == nameof(Person.Car));
            bool includeJob = context.Include.Includes.Any(p => p.Name == nameof(Person.Job));

            context.Filter.TryGetValue("HideLastName", out hideLastName);

            if (hideLastName.GetValueOrDefault() || !includeCar)
            {
                foreach (var person in data)
                {
                    if (hideLastName.GetValueOrDefault())
                    {
                        person.LastName = null;
                    }

                    if (!includeCar)
                        person.Car = null;

                    if (!includeJob)
                        person.Job = null;
                }
            }

            int? minAge;
            if (context.Filter.TryGetValue("MinAge", out minAge) && minAge.HasValue)
            {
                data = data.Where(person => person.Age >= minAge);
            }

            if (context.Pagination.PerPage.HasValue)
            {
                data = data.Take(context.Pagination.PerPage.Value);
            }

            return data;
        }

        [HttpGet]
        [HandlesQuery]
        [Route("query/manual-typed/people")]
        public IEnumerable<Person> ManualTypedQueryAndPaginatePeople([FromUri] PersonFilter filter)
        {
            IEnumerable<Person> data = GetPeopleNotRandom(10).ToList();
            if (filter?.HideLastName == true)
            {
                foreach (var person in data)
                {
                    person.LastName = null;
                }
            }

            if (filter?.MinAge != null)
            {
                data = data.Where(person => person.Age >= filter.MinAge);
            }

            return data;
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

        private static IEnumerable<Person> GetPeopleNotRandom(int count = 100)
        {
            for (var i = 0; i < count; i++)
            {
                var person = Get.Person(i.ToString());
                person.Age = i + 2;
                yield return person;
            }
        }
    }
}