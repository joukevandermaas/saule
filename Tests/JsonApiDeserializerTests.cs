using Newtonsoft.Json.Linq;
using Saule.Serialization;
using System.Linq;
using Tests.SampleModels;
using Xunit;

namespace Tests
{
    public class JsonApiDeserializerTests
    {
        private JToken _json;
        private Person _person;

        public JsonApiDeserializerTests()
        {
            _person = new Person(prefill: true);
            _person.Friends = new[] { new Person(prefill: true) };
            _json = new JsonApiSerializer().Serialize(
                new ApiResponse(_person, new PersonResource()), "/people/1/");
        }

        [Fact(DisplayName = "Deserializes id and attributes")]
        public void DeserializesAttributes()
        {
            var target = new JsonApiDeserializer();
            var result = target.Deserialize<Person>(_json);

            Assert.Equal(_person.Id, result.Id);
            Assert.Equal(_person.FirstName, result.FirstName);
            Assert.Equal(_person.LastName, result.LastName);
            Assert.Equal(_person.Age, result.Age);
        }

        [Fact(DisplayName = "Deserializes belongsTo relationships")]
        public void DeserializesBelongsToRelationships()
        {
            var target = new JsonApiDeserializer();
            var result = target.Deserialize<Person>(_json);
            var job = result.Job;

            Assert.Equal(_person.Job.Id, job.Id);
            Assert.Null(job.Name);
            Assert.Equal(0, job.NumberOfEmployees);
        }

        [Fact(DisplayName = "Deserializes hasMany relationships")]
        public void DeserializesHasManyRelationship()
        {
            var target = new JsonApiDeserializer();
            var result = target.Deserialize<Person>(_json);

            var expected = _person.Friends.Single();
            var actual = result.Friends.Single();

            Assert.Equal(expected.Id, actual.Id);
            Assert.Null(actual.FirstName);
            Assert.Null(actual.LastName);
            Assert.Equal(0, actual.Age);
            Assert.Null(actual.Job);
            Assert.Null(actual.Friends);
        }
    }
}
