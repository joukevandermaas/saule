using System;
using System.Collections.Generic;
using System.Linq;
using Saule.Http;
using System.Web.Http;
using Website.Models;
using Website.Resources;

namespace Website.Controllers
{
    [ReturnsResource(typeof(TestResource))]
    public class TestController : ApiController
    {
        [Route("api/tests/{id}")]
        public TestModel Get(string id)
        {
            return new TestModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = "my name"
            };
        }

        [Route("api/tests")]
        [Paginated(PerPage = 10)]
        public IEnumerable<TestModel> GetAll()
        {
            return GetModel().Take(105);
        }

        [Route("api/tests")]
        public TestModel Post(TestModel model)
        {
            return model;
        }

        private IEnumerable<TestModel> GetModel()
        {
            var i = 0;
            while (true)
            {
                yield return new TestModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"I'm test {i}"
                };
                i++;
            }
        }
    }
}