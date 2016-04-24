using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Serialization;
using System.Linq;
using System.Threading;
using Tests.Helpers;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class ResourceSerializerTests
    {
        private readonly ITestOutputHelper _output;

        private static Person DefaultObject { get; } = Get.Person();
        private static ApiResource DefaultResource { get; } = new PersonResource();
        private static Uri DefaultUrl { get; } = new Uri("http://example.com/");
        private static IUrlPathBuilder DefaultPathBuilder { get; } = new DefaultUrlPathBuilder("/api");

        public ResourceSerializerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Handles recursive properties on resource objects")]
        public void HandlesRecursiveProperties()
        {
            var firstModel = new Recursion.FirstModel();
            var secondModel = new Recursion.SecondModel();
            firstModel.Model = secondModel;
            secondModel.Model = firstModel;

            var target = new ResourceSerializer(firstModel, new Recursion.Resource(), 
                GetUri(id: "123"), DefaultPathBuilder, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(firstModel.Id, id);
        }

        [Fact(DisplayName = "Uses a property called 'Id' when none is specified for Ids")]
        public void UsesDefaultPropertyId()
        {
            var data = new PersonWithNoJob();
            var target = new ResourceSerializer(data, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(data.Id, id);
        }

        [Fact(DisplayName = "Uses specified property if it exists for Ids")]
        public void UsesSpecifiedPropertyId()
        {
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "abc"), DefaultPathBuilder, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(DefaultObject.Identifier, id);
        }

        [Fact(DisplayName = "Uses custom id property for urls")]
        public void UsesCustomIdInUrls()
        {
            var person = Get.Person(id: "abc");
            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "abc"), DefaultPathBuilder, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var job = result["data"]["relationships"]["job"]["links"];
            var friends = result["data"]["relationships"]["friends"]["links"];

            var self = result["links"]["self"].Value<Uri>().AbsolutePath;
            var jobSelf = job["self"].Value<Uri>().AbsolutePath;
            var jobRelated = job["related"].Value<Uri>().AbsolutePath;
            var friendsSelf = friends["self"].Value<Uri>().AbsolutePath;
            var friendsRelated = friends["related"].Value<Uri>().AbsolutePath;
            var included = result["included"][0]["links"]["self"].Value<Uri>().AbsolutePath;

            Assert.Equal("/api/people/abc", self);
            Assert.Equal("/api/people/abc/relationships/employer/", jobSelf);
            Assert.Equal("/api/people/abc/employer/", jobRelated);
            Assert.Equal("/api/people/abc/relationships/friends/", friendsSelf);
            Assert.Equal("/api/people/abc/friends/", friendsRelated);
            Assert.Equal("/api/corporations/456/", included);
        }

        [Fact(DisplayName = "Uses custom id property in collections")]
        public void UsesCustomIdInCollections()
        {
            var person = new Person(id: "abc", prefill: true)
            {
                Friends = new List<Person>
                {
                    new Person(id: "def", prefill: true),
                    new Person(id: "ghi", prefill: true),
                    new Person(id: "jkl", prefill: true),
                }
            };

            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "abc"), DefaultPathBuilder, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var ids = result["data"]["relationships"]["friends"]["data"].Select(t => t.Value<string>("id"));
            var expected = person.Friends.Select(p => p.Identifier);

            Assert.Equal(expected, ids);
        }

        [Fact(DisplayName = "Uses custom id property in relationships")]
        public void UsesCustomIdInRelationships()
        {
            var person = new PersonWithDifferentId(id: "abc", prefill: true);
            var resource = new PersonWithDifferentIdResource();
            var target = new ResourceSerializer(person, resource,
                GetUri(id: "abc"), DefaultPathBuilder, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"]["relationships"]["job"]["data"].Value<string>("id");

            Assert.Equal(person.Job.CompanyId, id);
        }

        [Fact(DisplayName = "Serializes all found attributes")]
        public void AttributesComplete()
        {
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var attributes = result["data"]["attributes"];
            Assert.Equal(DefaultObject.FirstName, attributes.Value<string>("first-name"));
            Assert.Equal(DefaultObject.LastName, attributes.Value<string>("last-name"));
            Assert.Equal(DefaultObject.Age, attributes.Value<int>("age"));
        }

        [Fact(DisplayName = "Serializes no extra properties")]
        public void AttributesSufficient()
        {
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var attributes = result["data"]["attributes"];
            Assert.True(attributes["numberOfLegs"] == null);
            Assert.Equal(3, attributes.Count());
        }

        [Fact(DisplayName = "Uses type name from model definition")]
        public void UsesTitle()
        {
            var company = Get.Company();
            var target = new ResourceSerializer(company, new CompanyResource(),
                GetUri("/corporations", "456"),
                DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal("corporation", result["data"]["type"]);
        }

        [Fact(DisplayName = "Throws exception when Id is missing")]
        public void ThrowsRightException()
        {
            var person = new PersonWithNoId();
            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null);

            Assert.Throws<JsonApiException>(() =>
            {
                target.Serialize();
            });
        }

        [Fact(DisplayName = "Serializes relationship data only if it exists")]
        public void SerializesRelationshipData()
        {
            var person = new PersonWithNoJob();
            var target = new ResourceSerializer(person, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null);
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
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;
            var job = included?[0];
            Assert.Equal(1, included?.Count);
            Assert.Equal("http://example.com/api/corporations/456/", included?[0]?["links"].Value<Uri>("self").ToString());

            Assert.Equal(DefaultObject.Job.Id, job?["id"]);
            Assert.NotNull(job?["attributes"]);
        }

        [Fact(DisplayName = "Handles null relationships and attributes correctly")]
        public void HandlesNullValues()
        {
            var person = new Person(id: "45");
            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "45"), DefaultPathBuilder, null);
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
            var people = Get.People(5);
            var target = new ResourceSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;
            var jobLinks = (result["data"] as JArray)?[0]["relationships"]["job"]["links"];

            Assert.Equal(1, included?.Count);
            Assert.Equal("/api/people/0/employer/", jobLinks?.Value<Uri>("related").AbsolutePath);
        }

        [Fact(DisplayName = "Document MUST contain at least one: data, errors, meta")]
        public void DocumentMustContainAtLeastOneDataOrErrorOrMeta()
        {
            var people = new Person[] { };
            var target = new ResourceSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.NotNull(result["data"]);
        }

        [Fact(DisplayName = "If a document does not contain a top-level data key, the included member MUST NOT be present either.")]
        public void DocumentMustNotContainIncludedForEmptySet()
        {
            var people = new Person[0];
            var target = new ResourceSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["included"]);
        }

        [Fact(DisplayName = "Handles null objects correctly")]
        public void HandlesNullResources()
        {
            var target = new ResourceSerializer(null, DefaultResource,
                GetUri(), DefaultPathBuilder, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(JTokenType.Null, result["data"].Type);
        }

        [Fact(DisplayName = "Supports Guids for ids")]
        public void SupportsGuidIds()
        {
            var guid = new GuidAsId();
            var serializer = new ResourceSerializer(guid, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null);

            var guidResult = serializer.Serialize();
            _output.WriteLine(guidResult.ToString());

            Assert.NotNull(guidResult["data"]["id"]);
            Assert.Equal(guid.Id, guidResult["data"].Value<Guid>("id"));
        }

        [Fact(DisplayName = "Supports Guids for ids in collections")]
        public void SupportsGuidIdsInCollections()
        {
            var guids = new [] { new GuidAsId(), new GuidAsId() };
            var serializer = new ResourceSerializer(guids, new PersonWithDefaultIdResource(),
                GetUri(id: "123"), DefaultPathBuilder, null);

            var guidsResult = serializer.Serialize();
            _output.WriteLine(guidsResult.ToString());

            Assert.NotEmpty(guidsResult["data"]);
        }

        [Fact(DisplayName = "Supports Guids for id in relations")]
        public void SupportsGuidsAsRelations()
        {
            var relatedToGuidId = new GuidAsRelation();
            var serializer = new ResourceSerializer( relatedToGuidId, new PersonWithGuidAsRelationsResource(),
                GetUri(id: "123"), DefaultPathBuilder, null);

            var result = serializer.Serialize();
            _output.WriteLine(result.ToString());

            Assert.NotNull(result["data"]["relationships"]["relation"]);
            Assert.NotNull(result["data"]["relationships"]["relations"]);
        }

        [Fact(DisplayName = "Does not serialize attributes that are not found")]
        public void SerializeOnlyWhatYouHave()
        {
            var company = new GuidAsId();
            var serializer = new ResourceSerializer(company, new CompanyResource(),
                GetUri(id: "123"), DefaultPathBuilder, null);

            var result = serializer.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["data"]["attributes"]["name"]);
            Assert.Null(result["data"]["attributes"]["location"]);
            Assert.Null(result["data"]["attributes"]["number-of-employees"]);
        }

        private static Uri GetUri(string path = "/people/", string id = null, string query = null)
        {
            var combined = "/api" + path;
            if (id != null) combined += id;
            if (query != null) combined += "?" + query;

            return new Uri(DefaultUrl, combined);
        }
    }
}