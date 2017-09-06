using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Saule.Serialization;
using System.Linq;
using Saule;
using Tests.Helpers;
using Tests.Models;
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
            _person = Get.Person(id: "hello");
            _person.Friends = Get.People(1);
            _person.FamilyMembers = Get.People(2);
            _people = Get.People(5).ToArray();
            var singleSerializer = new ResourceSerializer(
            _person, new PersonResource(), new Uri("http://example.com/people/1"),
            new DefaultUrlPathBuilder(), null, null);
            var multiSerializer = new ResourceSerializer(
                _people, new PersonResource(), new Uri("http://example.com/people/"),
                new DefaultUrlPathBuilder(), null, null);

            _singleJson = JToken.Parse(singleSerializer.Serialize().ToString());
            _collectionJson = JToken.Parse(multiSerializer.Serialize().ToString());
        }

        [Fact(DisplayName = "Deserializes id and attributes")]
        public void DeserializesAttributes()
        {
            var target = new ResourceDeserializer(_singleJson, typeof(Person));
            var result = target.Deserialize() as Person;

            Assert.Equal(_person.Identifier, result?.Identifier);
            Assert.Equal(_person.FirstName, result?.FirstName);
            Assert.Equal(_person.LastName, result?.LastName);
            Assert.Equal(_person.Age, result?.Age);
        }

        [Fact(DisplayName = "Deserializes if id does not exist")]
        public void DeserializesWithoutId()
        {
            (_singleJson["data"] as JObject)?.Property("id").Remove();
            var target = new ResourceDeserializer(_singleJson, typeof(Person));
            var result = target.Deserialize() as Person;

            Assert.Equal(null, result?.Identifier);
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

        [Fact(DisplayName = "Deserializes belongsTo relationships with data: null")]
        public void DeserializesBelongsToRelationshipsWithNullData()
        {
            var person = JToken.Parse("{'data': {'relationships': {'job': {data: null}}}}");
            var target = new ResourceDeserializer(person, typeof(Person));
            var result = target.Deserialize() as Person;

            var job = result?.Job;

            Assert.Null(job);
        }

        [Fact(DisplayName = "Deserializes hasMany relationships")]
        public void DeserializesHasManyRelationship()
        {
            var target = new ResourceDeserializer(_singleJson, typeof(Person));
            var result = target.Deserialize() as Person;

            var expectedFriend = _person.Friends.Single();
            var actualFriend = result?.Friends.Single();

            Assert.Equal(expectedFriend.Identifier, actualFriend?.Identifier);
            Assert.Null(actualFriend?.FirstName);
            Assert.Null(actualFriend?.LastName);
            Assert.Equal(default(int?), actualFriend?.Age);
            Assert.Null(actualFriend?.Job);
            Assert.Null(actualFriend?.Friends);

            var expectedFamilyMembers = _person.FamilyMembers.ToArray();
            var actualFamilyMembers = result?.FamilyMembers?.ToArray();

            Assert.NotNull(actualFamilyMembers);
            Assert.Equal(expectedFamilyMembers.Length, actualFamilyMembers.Length);
            expectedFamilyMembers.Zip(actualFamilyMembers, (expected, actual) =>
            {
                Assert.Equal(expected.Identifier, actual.Identifier);
                return true;
            });
        }

        [Fact(DisplayName = "Deserializes enumerables properly")]
        public void DeserializesEnumerables()
        {
            var target = new ResourceDeserializer(_collectionJson, typeof(Person[]));
            var result = target.Deserialize() as Person[];

            Assert.Equal(_people.Length, result?.Length);
            for (var i = 0; i < _people.Length; i++)
            {
                Assert.Equal(_people[i].Identifier, result?[i].Identifier);
            }
        }

        [Fact(DisplayName = "A document MUST contain at least one of the following top-level members: 'data', 'errors', 'meta'")]
        public void ValidatesRequiredTopLevelMembers()
        {
            var content = FileAsJson("no-data-errors-meta.json");
            var target = new ResourceDeserializer(content, typeof(Person));

            Assert.Throws<JsonApiException>(() => target.Deserialize());
        }

        [Fact(DisplayName = "The members 'data' and 'errors' MUST NOT coexist in the same document")]
        public void ValidatesMutuallyExclusiveTopLevelMembers()
        {
            var content = FileAsJson("errors-and-data.json");
            var target = new ResourceDeserializer(content, typeof(Person));

            Assert.Throws<JsonApiException>(() => target.Deserialize());
        }

        [Fact(DisplayName = "If a document does not contain a top-level 'data' key, the 'included' member MUST NOT be present either")]
        public void ValidatesNoIncludedWithoutData()
        {
            var content = FileAsJson("included-no-data.json");
            var target = new ResourceDeserializer(content, typeof(Person));

            Assert.Throws<JsonApiException>(() => target.Deserialize());
        }

        [Fact(DisplayName = "No other top-level members are allowed")]
        public void ValidatesTopLevelMemberExclusivity()
        {
            var content = FileAsJson("invalid-top-level.json");
            var target = new ResourceDeserializer(content, typeof(Person));

            Assert.Throws<JsonApiException>(() => target.Deserialize());
        }

        [Fact(DisplayName = "A JSON object MUST be at the root of every JSON API request containing data")]
        public void ValidatesRootIsObject()
        {
            var content = FileAsJson("no-object-root.json");
            var target = new ResourceDeserializer(content, typeof(Person));

            Assert.Throws<JsonApiException>(() => target.Deserialize());
        }

        private static JToken FileAsJson(string filename) => JToken.Parse(File.ReadAllText($"../../../Assets/{filename}"));
    }
}