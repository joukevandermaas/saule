using Newtonsoft.Json.Linq;
using Saule;
using Saule.Serialization;
using System.Linq;
using Tests.SampleModels;
using Xunit;

namespace Tests.Serialization
{
    public class ResourceSerializerTests
    {
        [Fact(DisplayName = "Serializes all found attributes")]
        public void AttributesComplete()
        {
            var person = new Person(prefill: true);
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");
            var result = target.Serialize();

            var attributes = result["data"]["attributes"];
            Assert.Equal(person.FirstName, attributes.Value<string>("firstName"));
            Assert.Equal(person.LastName, attributes.Value<string>("lastName"));
            Assert.Equal(person.Age, attributes.Value<int>("age"));
        }

        [Fact(DisplayName = "Serializes no extra properties")]
        public void AttributesSufficient()
        {
            var person = new Person(prefill: true);
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");
            var result = target.Serialize();

            var attributes = result["data"]["attributes"];
            Assert.True(attributes["numberOfLegs"] == null);
            Assert.Equal(3, attributes.Count());
        }

        [Fact(DisplayName = "Uses type name from model definition")]
        public void UsesTitle()
        {
            var company = new Company(prefill: true);
            var target = new ResourceSerializer(company, new CompanyResource(), "/companies/1/");
            var result = target.Serialize();

            Assert.Equal("coorporation", result["data"]["type"]);
        }

        [Fact(DisplayName = "Adds top level self link")]
        public void SelfLink()
        {
            var person = new Person(prefill: true);
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");
            var result = target.Serialize();

            var selfLink = result["links"]?["self"];

            Assert.Equal("/people/1", selfLink);
        }

        [Fact(DisplayName = "Serializes relationships' links")]
        public void SerializesRelationshipLinks()
        {
            var person = new Person(prefill: true);
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");
            var result = target.Serialize();

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Equal("/people/1/employer", job["links"].Value<string>("related"));
            Assert.Equal("/people/1/relationships/employer", job["links"].Value<string>("self"));

            Assert.Equal("/people/1/friends", friends["links"].Value<string>("related"));
            Assert.Equal("/people/1/relationships/friends", friends["links"].Value<string>("self"));
        }

        [Fact(DisplayName = "Builds absolute links correctly")]
        public void BuildsRightLinks()
        {
            var person = new Person(prefill: true);
            var target = new ResourceSerializer(person, new PersonResource(), "http://example.com/people/1/");
            var result = target.Serialize();

            var job = result["data"]["relationships"]["job"];

            Assert.Equal("http://example.com/people/1/employer", job["links"].Value<string>("related"));
            Assert.Equal("http://example.com/people/1/relationships/employer", job["links"].Value<string>("self"));
        }

        [Fact(DisplayName = "Throws exception when Id is missing")]
        public void ThrowsRightException()
        {
            var person = new PersonWithNoId();
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");

            Assert.Throws<JsonApiException>(() =>
            {
                target.Serialize();
            });
        }

        [Fact(DisplayName = "Serializes relationship data only if it exists")]
        public void SerializesRelationshipData()
        {
            var person = new PersonWithNoJob();
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");
            var result = target.Serialize();

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Null(job["data"]);
            Assert.NotNull(friends["data"]);
        }

        [Fact(DisplayName = "Serializes relationship data into 'included' key")]
        public void IncludesRelationshipData()
        {
            var person = new Person(prefill: true);
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");
            var result = target.Serialize();

            var included = result["included"] as JArray;
            var job = included[0];
            Assert.Equal(1, included.Count);

            Assert.Equal(person.Job.Id, job["id"]);
            Assert.NotNull(job["attributes"]);
        }

        [Fact(DisplayName = "Handles null values correctly")]
        public void HandlesNullValues()
        {
            var person = new Person { Id = "45" };
            var target = new ResourceSerializer(person, new PersonResource(), "/people/1/");
            var result = target.Serialize();

            var relationships = result["data"]["relationships"];
            var attributes = result["data"]["attributes"];

            Assert.NotNull(attributes["firstName"]);
            Assert.NotNull(attributes["lastName"]);
            Assert.NotNull(attributes["age"]);

            Assert.Null(relationships["job"]["data"]);
            Assert.Null(relationships["friends"]["data"]);
        }

        [Fact(DisplayName = "Serializes enumerables properly")]
        public void SerializesEnumerables()
        {
            var people = new Person[]
            {
                new Person(id: "a", prefill: true),
                new Person(id: "b", prefill: true),
                new Person(id: "c", prefill: true),
                new Person(id: "d", prefill: true)
            };
            var target = new ResourceSerializer(people, new PersonResource(), "/people/");
            var result = target.Serialize();

            var included = result["included"] as JArray;
            var jobLinks = (result["data"] as JArray)?[0]["relationships"]["job"]["links"];

            Assert.Equal(1, included?.Count);
            Assert.Equal("/people/a/employer", jobLinks?.Value<string>("related"));
        }
    }
}