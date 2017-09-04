using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        [HttpGet]
        [Route("api/broken/errors")]
        [ReturnsResource(typeof(PersonResource))]
        public HttpResponseMessage TwoErrors()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, new List<HttpError>()
            {
                new HttpError("Error 1")
                {
                    ExceptionType = "Type 1"
                },
                new HttpError("Error 2")
                {
                    ExceptionType = "Type 2"
                }
            });
        }
    }
}
