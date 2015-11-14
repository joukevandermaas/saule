using System.Web.Http;
using Tests.Models;

namespace Tests.Controllers
{
    public class BrokenController : ApiController
    {
        [HttpGet]
        public Person GetPerson(string id)
        {
            return new Person(prefill: true, id: id);
        }
    }
}
