using Saule.Http;
using System.Web.Http;
using Website.Models;

namespace Website.Controllers
{
    public class TestController : ApiController
    {
        [Route("test")]
        [ApiResource(typeof(TestResource))]
        public TestModel Get()
        {
            return new TestModel
            {
                Id = "my id",
                Name = "my name"
            };
        }

        [Route("test")]
        [ApiResource(typeof(TestResource))]
        public TestModel Post(TestModel model)
        {
            return model;
        }
    }
}