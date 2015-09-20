using System;
using Newtonsoft.Json.Linq;
using Saule.Serialization;
using System.Linq;
using Tests.Helpers;
using Xunit;

namespace Tests.Serialization
{
    public class ResourceDeserializerTests
    {
        private readonly JToken _singleJson;
        private readonly JToken _collectionJson;
        private readonly Person _person;
        private readonly Person[] _people;

        public ResourceDeserializerTests()
        {
            _person = new Person(prefill: true, id: "123")
            {
                Friends = new[] { new Person(prefill: true, id: "456")
                {
                    FirstName = "Sara",
                    LastName =  "Jones",
                    Age = 38
                } }
            };
            _people = new[]
            {
                new Person(id: "a", prefill: true),
                new Person(id: "b", prefill: true),
                new Person(id: "c", prefill: true),
                new Person(id: "d", prefill: true)
            };

            var singleSerializer = new ResourceSerializer(
                _person, new PersonResource(), new Uri("http://example.com/people/1"));
            var multiSerializer = new ResourceSerializer(
                _people, new PersonResource(), new Uri("http://example.com/people/"));

            _singleJson = singleSerializer.Serialize();
            _collectionJson = multiSerializer.Serialize();
        }

        [Fact(DisplayName = "Deserializes id and attributes")]
        public void DeserializesAttributes()
        {
            var target = new ResourceDeserializer(_singleJson, typeof(Person));
            var result = target.Deserialize() as Person;

            Assert.Equal(_person.Id, result?.Id);
            Assert.Equal(_person.FirstName, result?.FirstName);
            Assert.Equal(_person.LastName, result?.LastName);
            Assert.Equal(_person.Age, result?.Age);
        }

        [Fact(DisplayName = "Deserializes belongsTo relationships")]
        public void DeserializesBelongsToRelationships()
        {
            var target = new ResourceDeserializer(_singleJson, typeof(Person));
            var result = target.Deserialize() as Person;
            var job = result?.Job;

            Assert.Equal(_person.Job.Id, job?.Id);
            Assert.Null(job?.Name);
            Assert.Equal(0, job?.NumberOfEmployees);
        }

        [Fact(DisplayName = "Deserializes hasMany relationships")]
        public void DeserializesHasManyRelationship()
        {
            var target = new ResourceDeserializer(_singleJson, typeof(Person));
            var result = target.Deserialize() as Person;

            var expected = _person.Friends.Single();
            var actual = result?.Friends.Single();

            Assert.Equal(expected.Id, actual?.Id);
            Assert.Null(actual?.FirstName);
            Assert.Null(actual?.LastName);
            Assert.Equal(0, actual?.Age);
            Assert.Null(actual?.Job);
            Assert.Null(actual?.Friends);
        }

        [Fact(DisplayName = "Deserializes enumerables properly")]
        public void DeserializesEnumerables()
        {
            var target = new ResourceDeserializer(_collectionJson, typeof(Person[]));
            var result = target.Deserialize() as Person[];

            Assert.Equal(_people.Length, result?.Length);
            for (var i = 0; i < _people.Length; i++)
            {
                Assert.Equal(_people[i].Id, result?[i].Id);
            }
        }
    }
}