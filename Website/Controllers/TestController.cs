using Saule.Http;
using System;
using System.Web.Http;
using Website.Models;

namespace Website.Controllers
{
    [ApiResource(typeof(TestResource))]
    public class TestController : ApiController
    {
        [Route("test")]
        public TestModel Get()
        {
            throw new ArgumentException("Hello", "test");
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