using Saule.Http;
using System.Web.Http;
using Website.Models;
using Website.Resources;

namespace Website.Controllers
{
    [ReturnsResource(typeof(TestResource))]
    public class TestController : ApiController
    {
        [Route("test")]
        public TestModel Get()
        {
            return new TestModel
            {
                Id = "my id",
                Name = "my name"
            };
        }

        [Route("test")]
        public TestModel Post(TestModel model)
        {
            return model;
        }
    }
}