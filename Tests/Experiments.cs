using Saule.Serialization;
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
            var json = new ResourceSerializer(person, new PersonResource(), "/").Serialize();
            var target = new ResourceDeserializer();

            var result = target.Deserialize(json, typeof(Person));
        }
    }
}