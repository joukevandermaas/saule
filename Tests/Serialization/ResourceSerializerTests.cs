using System;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Serialization;
using System.Linq;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class ResourceSerializerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ResourceSerializer _target;
        private readonly Person _person;

        public ResourceSerializerTests(ITestOutputHelper output)
        {
            _output = output;
            _person = new Person(prefill: true);
            _target = new ResourceSerializer(
                _person, new PersonResource(), new Uri("http://example.com/people/123/"),
                new DefaultUrlPathBuilder(), null);
        }

        [Fact(DisplayName = "Serializes all found attributes")]
        public void AttributesComplete()
        {
            var result = _target.Serialize();
            _output.WriteLine(result.ToString());

            var attributes = result["data"]["attributes"];
            Assert.Equal(_person.FirstName, attributes.Value<string>("first-name"));
            Assert.Equal(_person.LastName, attributes.Value<string>("last-name"));
            Assert.Equal(_person.Age, attributes.Value<int>("age"));
        }

        [Fact(DisplayName = "Serializes no extra properties")]
        public void AttributesSufficient()
        {
            var result = _target.Serialize();
            _output.WriteLine(result.ToString());

            var attributes = result["data"]["attributes"];
            Assert.True(attributes["numberOfLegs"] == null);
            Assert.Equal(3, attributes.Count());
        }

        [Fact(DisplayName = "Uses type name from model definition")]
        public void UsesTitle()
        {
            var company = new Company(prefill: true);
            var target = new ResourceSerializer(company,
                new CompanyResource(), new Uri("http://example.com/companies/1/"),
                new DefaultUrlPathBuilder(), null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal("corporation", result["data"]["type"]);
        }

        [Fact(DisplayName = "Throws exception when Id is missing")]
        public void ThrowsRightException()
        {
            var person = new PersonWithNoId();
            var target = new ResourceSerializer(person, new PersonResource(),
                new Uri("http://example.com/people/1/"),
                new DefaultUrlPathBuilder(), null);

            Assert.Throws<JsonApiException>(() =>
            {
                target.Serialize();
            });
        }

        [Fact(DisplayName = "Serializes relationship data only if it exists")]
        public void SerializesRelationshipData()
        {
            var person = new PersonWithNoJob();
            var target = new ResourceSerializer(person, new PersonResource(),
                new Uri("http://example.com/people/123"),
                new DefaultUrlPathBuilder(), null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Null(job["data"]);
            Assert.NotNull(friends["data"]);
        }

        [Fact(DisplayName = "Serializes relationship data into 'included' key")]
        public void IncludesRelationshipData()
        {
            var result = _target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;
            var job = included?[0];
            Assert.Equal(1, included?.Count);
            Assert.Equal("http://example.com/corporations/456/", included?[0]?["links"].Value<Uri>("self").ToString());

            Assert.Equal(_person.Job.Id, job?["id"]);
            Assert.NotNull(job?["attributes"]);
        }

        [Fact(DisplayName = "Handles null relationships and attributes correctly")]
        public void HandlesNullValues()
        {
            var person = new Person { Id = "45" };
            var target = new ResourceSerializer(
                person, new PersonResource(),
                new Uri("http://example.com/people/1/"),
                new DefaultUrlPathBuilder(), null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var attributes = result["data"]["attributes"];

            Assert.NotNull(attributes["first-name"]);
            Assert.NotNull(attributes["last-name"]);
            Assert.NotNull(attributes["age"]);

            Assert.Null(relationships["job"]["data"]);
            Assert.Null(relationships["friends"]["data"]);
        }

        [Fact(DisplayName = "Serializes enumerables properly")]
        public void SerializesEnumerables()
        {
            var people = new[]
            {
                new Person(id: "a", prefill: true),
                new Person(id: "b", prefill: true),
                new Person(id: "c", prefill: true),
                new Person(id: "d", prefill: true)
            };
            var target = new ResourceSerializer(people,
                new PersonResource(),
                new Uri("http://example.com/people/"),
                new DefaultUrlPathBuilder(), null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;
            var jobLinks = (result["data"] as JArray)?[0]["relationships"]["job"]["links"];

            Assert.Equal(1, included?.Count);
            Assert.Equal("/people/a/employer/", jobLinks?.Value<Uri>("related").AbsolutePath);
        }

        [Fact(DisplayName = "Document MUST contain at least one: data, errors, meta")]
        public void DocumentMustContainAtLeastOneDataOrErrorOrMeta()
        {
            var people = new Person[] { };
            var target = new ResourceSerializer(people, new PersonResource(),
                new Uri("http://example.com/people/"), new DefaultUrlPathBuilder(), null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.NotNull(result["data"]);
        }

        [Fact(DisplayName = "If a document does not contain a top-level data key, the included member MUST NOT be present either.")]
        public void DocumentMustNotContainIncludedForEmptySet()
        {
            var people = new Person[] { };
            var target = new ResourceSerializer(people, new PersonResource(),
                new Uri("http://example.com/people/"),
                new DefaultUrlPathBuilder(), null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["included"]);
        }

        [Fact(DisplayName = "Handles null objects correctly")]
        public void HandlesNullResources()
        {
            var target = new ResourceSerializer(null,
                new PersonResource(), new Uri("http://example.com/people"),
                new DefaultUrlPathBuilder(), null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(JTokenType.Null, result["data"].Type);
        }

        [Fact(DisplayName = "Supports Guids for ids")]
        public void SupportsGuidIds()
        {
            var guid = new GuidAsId();
            var serializer = new ResourceSerializer(guid, new PersonResource(), new Uri("http://example.com/people/1"),
                new DefaultUrlPathBuilder(), null);

            var guidResult = serializer.Serialize();
            _output.WriteLine(guidResult.ToString());

            Assert.NotNull(guidResult["data"]["id"]);
            Assert.Equal(guid.Id, guidResult["data"].Value<Guid>("id"));
        }

        [Fact(DisplayName = "Does not serialize attributes that are not found")]
        public void SerializeOnlyWhatYouHave()
        {
            var person = new GuidAsId();
            var serializer = new ResourceSerializer(person, new PersonResource(), new Uri("http://example.com/people/1"),
                new DefaultUrlPathBuilder(), null);

            var result = serializer.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["data"]["attributes"]["first-name"]);
            Assert.Null(result["data"]["attributes"]["last-name"]);
            Assert.Null(result["data"]["attributes"]["age"]);
        }
    }
}