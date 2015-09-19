using System;
using System.Collections.Generic;
using Saule.Http;
using System.Web.Http;
using Website.Models;
using Website.Resources;

namespace Website.Controllers
{
    [ReturnsResource(typeof(TestResource))]
    public class TestController : ApiController
    {
        [Route("api/test/{id}")]
        public TestModel Get(string id)
        {
            return new TestModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = "my name"
            };
        }

        [Route("tests")]
        public IEnumerable<TestModel> GetAll()
        {
            yield return new TestModel
            {
                Id = Guid.NewGuid().ToString(),
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