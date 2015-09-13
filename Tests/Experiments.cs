using Newtonsoft.Json.Linq;
using Saule.Serialization;
using System.Collections.Generic;
using Tests.SampleModels;
using Xunit;

namespace Tests
{
    public class Experiments
    {
        [Fact]
        public void Run()
        {
            var person = new Person(false)
            {
                Id = "aaa",
                FirstName = "Adam",
                LastName = "Johnson",
                Age = 54,

                Friends = new[] { new Person(prefill: true) },
                Job = new Company(prefill: true)
            };
            var json = new JsonApiSerializer().Serialize(new ApiResponse(person, new PersonResource()), "/");
            var target = new JsonApiDeserializer();

            var result = target.Deserialize<Person>(json);
        }
    }
}
