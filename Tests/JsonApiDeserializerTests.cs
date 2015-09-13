using Newtonsoft.Json.Linq;
using Saule.Serialization;
using System.Linq;
using Tests.SampleModels;
using Xunit;

namespace Tests
{
    public class JsonApiDeserializerTests
    {
        private JToken _singleJson;
        private JToken _collectionJson;
        private Person _person;
        private Person[] _people;

        public JsonApiDeserializerTests()
        {
            _person = new Person(prefill: true);
            _person.Friends = new[] { new Person(prefill: true) };
            _people = new Person[]
            {
                new Person(id: "a", prefill: true),
                new Person(id: "b", prefill: true),
                new Person(id: "c", prefill: true),
                new Person(id: "d", prefill: true)
            };

            var serializer = new JsonApiSerializer();

            _singleJson = serializer.Serialize(
                new ApiResponse(_person, new PersonResource()), "/people/1/");
            _collectionJson = serializer.Serialize(
                new ApiResponse(_people, new PersonResource()), "/people/");
        }

        [Fact(DisplayName = "Deserializes id and attributes")]
        public void DeserializesAttributes()
        {
            var target = new JsonApiDeserializer();
            var result = target.Deserialize(_singleJson, typeof(Person)) as Person;

            Assert.Equal(_person.Id, result.Id);
            Assert.Equal(_person.FirstName, result.FirstName);
            Assert.Equal(_person.LastName, result.LastName);
            Assert.Equal(_person.Age, result.Age);
        }

        [Fact(DisplayName = "Deserializes belongsTo relationships")]
        public void DeserializesBelongsToRelationships()
        {
            var target = new JsonApiDeserializer();
            var result = target.Deserialize(_singleJson, typeof(Person)) as Person;
            var job = result.Job;

            Assert.Equal(_person.Job.Id, job.Id);
            Assert.Null(job.Name);
            Assert.Equal(0, job.NumberOfEmployees);
        }

        [Fact(DisplayName = "Deserializes hasMany relationships")]
        public void DeserializesHasManyRelationship()
        {
            var target = new JsonApiDeserializer();
            var result = target.Deserialize(_singleJson, typeof(Person)) as Person;

            var expected = _person.Friends.Single();
            var actual = result.Friends.Single();

            Assert.Equal(expected.Id, actual.Id);
            Assert.Null(actual.FirstName);
            Assert.Null(actual.LastName);
            Assert.Equal(0, actual.Age);
            Assert.Null(actual.Job);
            Assert.Null(actual.Friends);
        }

        [Fact(DisplayName = "Deserializes enumerables properly")]
        public void DeserializesEnumerables()
        {
            var target = new JsonApiDeserializer();
            var result = target.Deserialize(_collectionJson, typeof(Person[])) as Person[];

            Assert.Equal(_people.Length, result.Length);
            for (var i = 0; i < _people.Length; i++)
            {
                Assert.Equal(_people[i].Id, result[i].Id);
            }
        }
    }
}