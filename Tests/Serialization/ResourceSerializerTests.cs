using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Serialization;
using System.Linq;
using Tests.Helpers;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;
using Saule.Queries.Including;
using Saule.Queries.Pagination;

namespace Tests.Serialization
{
    [Trait("Serializer", "ResourceSerializer")]
    public class ResourceSerializerTests
    {
        internal readonly ITestOutputHelper _output;

        internal static Person DefaultObject { get; } = Get.Person();
        internal static ApiResource DefaultResource { get; } = new PersonResource();
        internal static Uri DefaultUrl { get; } = new Uri("http://example.com/");
        internal static IUrlPathBuilder DefaultPathBuilder { get; } = new DefaultUrlPathBuilder("/api");

        public ResourceSerializerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        internal virtual IResourceSerializer GetSerializer(
            object value,
            ApiResource type,
            Uri baseUrl,
            IUrlPathBuilder urlBuilder,
            PaginationContext paginationContext,
            IncludingContext includingContext)
        {
            _output.WriteLine("Serializer type: ResourceSerializer");
            return new ResourceSerializer(value, type, baseUrl, urlBuilder, paginationContext, includingContext);
        }

        [Fact(DisplayName = "Handles recursive properties on resource objects")]
        public virtual void HandlesRecursiveProperties()
        {
            var firstModel = new Recursion.FirstModel();
            var secondModel = new Recursion.SecondModel();
            var thirdModel = new Recursion.ThirdModel();
            var fourthModel = new Recursion.FourthModel();
            firstModel.Child = secondModel;
            secondModel.Parent = firstModel;
            secondModel.Child = thirdModel;
            thirdModel.Parent = secondModel;
            thirdModel.Child = fourthModel;
            fourthModel.Parent = thirdModel;

            var target = GetSerializer(firstModel, new Recursion.FirstModelResource(), 
                GetUri(id: firstModel.Id), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(firstModel.Id, id);
            Assert.Equal(3, (result["included"] as JArray).Count);
        }

        [Fact(DisplayName = "Uses a property called 'Id' when none is specified for Ids")]
        public virtual void UsesDefaultPropertyId()
        {
            var data = new PersonWithNoJob();
            var target = GetSerializer(data, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(data.Id, id);
        }

        [Fact(DisplayName = "Uses specified property if it exists for Ids")]
        public virtual void UsesSpecifiedPropertyId()
        {
            var target = GetSerializer(DefaultObject, DefaultResource,
                GetUri(id: "abc"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(DefaultObject.Identifier, id);
        }

        [Fact(DisplayName = "Uses custom id property for urls")]
        public virtual void UsesCustomIdInUrls()
        {
            var person = Get.Person(id: "abc");
            person.Friends = Get.People(1);
            var target = GetSerializer(person, DefaultResource,
                GetUri(id: "abc"), DefaultPathBuilder, null, null);

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
        public virtual void UsesCustomIdInCollections()
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

            var target = GetSerializer(person, DefaultResource,
                GetUri(id: "abc"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var ids = result["data"]["relationships"]["friends"]["data"].Select(t => t.Value<string>("id"));
            var expected = person.Friends.Select(p => p.Identifier);

            Assert.Equal(expected, ids);
        }

        [Fact(DisplayName = "Uses custom id property in relationships")]
        public virtual void UsesCustomIdInRelationships()
        {
            var person = new PersonWithDifferentId(id: "abc", prefill: true);
            var resource = new PersonWithDifferentIdResource();
            var target = GetSerializer(person, resource,
                GetUri(id: "abc"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"]["relationships"]["job"]["data"].Value<string>("id");

            Assert.Equal(person.Job.CompanyId, id);
        }

        [Fact(DisplayName = "Serializes all found attributes")]
        public virtual void AttributesComplete()
        {
            var target = GetSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var attributes = result["data"]["attributes"];
            Assert.Equal(DefaultObject.FirstName, attributes.Value<string>("first-name"));
            Assert.Equal(DefaultObject.LastName, attributes.Value<string>("last-name"));
            Assert.Equal(DefaultObject.Age, attributes.Value<int>("age"));
            Assert.Equal(DefaultObject.Address.StreetName, attributes["address"].Value<string>("street-name"));
            Assert.Equal(DefaultObject.Address.ZipCode, attributes["address"].Value<string>("zip-code"));
        }

        [Fact(DisplayName = "Serializes no extra properties")]
        public virtual void AttributesSufficient()
        {
            var target = GetSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var attributes = result["data"]["attributes"];
            Assert.True(attributes["numberOfLegs"] == null, null);
            Assert.Equal(4, attributes.Count());
        }

        [Fact(DisplayName = "Uses type name from model definition")]
        public virtual void UsesTitle()
        {
            var company = Get.Company();
            var target = GetSerializer(company, new CompanyResource(),
                GetUri("/corporations", "456"),
                DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal("corporation", result["data"]["type"]);
        }

        [Fact(DisplayName = "Throws exception when Id is missing")]
        public virtual void ThrowsRightException()
        {
            var person = new PersonWithNoId();
            var target = GetSerializer(person, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            Assert.Throws<JsonApiException>(() =>
            {
                target.Serialize();
            });
        }

        [Fact(DisplayName = "Serializes relationship data only if it exists")]
        public virtual void SerializesRelationshipData()
        {
            var person = new PersonWithNoJob();
            var target = GetSerializer(person, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Null(job["data"]);
            Assert.NotNull(friends);
        }

        [Fact(DisplayName = "Serializes relationship data into 'included' key")]
        public virtual void IncludesRelationshipData()
        {
            var target = GetSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;
            var job = included?[0];
            Assert.Equal(2, included?.Count);
            Assert.Equal("http://example.com/api/corporations/456/", included?[0]?["links"].Value<Uri>("self").ToString());

            Assert.Equal(DefaultObject.Job.Id, job?["id"]);
            Assert.NotNull(job?["attributes"]);
        }

        [Fact(DisplayName = "Do not serialize relationship data into 'included' key when includedDefault set to false")]
        public virtual void NoIncludedRelationshipData()
        {
            var includes = new IncludingContext();
            includes.DisableDefaultIncluded = true;
            var target = GetSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, includes);
            
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            Assert.Null(included);
        }

        [Fact(DisplayName = "Relationships of included resources have correct URLs")]
        public virtual void IncludedResourceRelationshipURLsAreCorrect()
        {
            var person = new Person(true)
            {
                Job = new CompanyWithCustomers(true)
            };

            var target = GetSerializer(person, new PersonWithCompanyWithCustomersResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            Assert.Equal("http://example.com/api/corporations/456/relationships/customers/", included?[0]?["relationships"]?["customers"]?["links"]?.Value<Uri>("self")?.ToString()); 
        }

        [Fact(DisplayName = "Explicitly included resource referenced in multiple resources is only included once")]
        public virtual void IncludedResourceOnlyOnce()
        {
            var job = new CompanyWithCustomers(id: "457", prefill: true);
            var person = new Person(true)
            {
                Friends = new List<Person>
                {
                    new Person(id: "124", prefill: true) {
                        Job = job
                    },
                    new Person(id: "125", prefill: true) {
                        Job = job
                    }
                }
            };

            var include = new IncludingContext(GetQuery("include", "friends.job"));
            var target = GetSerializer(person, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, include);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            Assert.Equal(4, included.Count);
        }

        [Fact(DisplayName = "Handles null relationships and attributes correctly")]
        public virtual void HandlesNullValues()
        {
            var person = new Person(id: "45");
            var target = GetSerializer(person, DefaultResource,
                GetUri(id: "45"), DefaultPathBuilder, null, null);
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
        public virtual void SerializesEnumerables()
        {
            var people = Get.People(5);
            var target = GetSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;
            var jobLinks = (result["data"] as JArray)?[0]["relationships"]["job"]["links"];

            Assert.Equal(2, included?.Count);
            Assert.Equal("/api/people/0/employer/", jobLinks?.Value<Uri>("related").AbsolutePath);
        }

        [Fact(DisplayName = "Document MUST contain at least one: data, errors, meta")]
        public virtual void DocumentMustContainAtLeastOneDataOrErrorOrMeta()
        {
            var people = new Person[] { };
            var target = GetSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.NotNull(result["data"]);
        }

        [Fact(DisplayName = "If a document does not contain a top-level data key, the included member MUST NOT be present either.")]
        public virtual void DocumentMustNotContainIncludedForEmptySet()
        {
            var people = new Person[0];
            var target = GetSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["included"]);
        }

        [Fact(DisplayName = "Handles null objects correctly")]
        public virtual void HandlesNullResources()
        {
            var target = GetSerializer(null, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(JTokenType.Null, result["data"].Type);
        }

        [Fact(DisplayName = "Supports Guids for ids")]
        public virtual void SupportsGuidIds()
        {
            var guid = new GuidAsId();
            var serializer = GetSerializer(guid, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var guidResult = serializer.Serialize();
            _output.WriteLine(guidResult.ToString());

            Assert.NotNull(guidResult["data"]["id"]);
            Assert.Equal(guid.Id, guidResult["data"].Value<Guid>("id"));
        }

        [Fact(DisplayName = "Supports Guids for ids in collections")]
        public virtual void SupportsGuidIdsInCollections()
        {
            var guids = new [] { new GuidAsId(), new GuidAsId() };
            var serializer = GetSerializer(guids, new PersonWithDefaultIdResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var guidsResult = serializer.Serialize();
            _output.WriteLine(guidsResult.ToString());

            Assert.NotEmpty(guidsResult["data"]);
        }

        [Fact(DisplayName = "Supports Guids for id in relations")]
        public virtual void SupportsGuidsAsRelations()
        {
            var relatedToGuidId = new GuidAsRelation();
            var serializer = GetSerializer( relatedToGuidId, new PersonWithGuidAsRelationsResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var result = serializer.Serialize();
            _output.WriteLine(result.ToString());

            Assert.NotNull(result["data"]["relationships"]["relation"]);
            Assert.NotNull(result["data"]["relationships"]["relations"]);
        }

        [Fact(DisplayName = "Does not serialize attributes that are not found")]
        public virtual void SerializeOnlyWhatYouHave()
        {
            var company = new GuidAsId();
            var serializer = GetSerializer(company, new CompanyResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var result = serializer.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["data"]["attributes"]["name"]);
            Assert.Null(result["data"]["attributes"]["location"]);
            Assert.Null(result["data"]["attributes"]["number-of-employees"]);
        }

        [Fact(DisplayName = "A compound document MUST NOT include more than one resource object for each type and id pair")]
        public virtual void ResourceObjectsAreNotDuplicated()
        {
            var personA = new Person(false, "1");
            var personB = new Person(false, "2");
            personA.Friends = new Person[] { personB };
            personB.Friends = new Person[] { personA };

            var people = new Person[] { personA, personB };

            var target = GetSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());
            var data = result["data"] as JArray;
            var included = result["included"] as JArray;

            if (included == null)
                return;

            var combined = data.Concat(included);

            var duplicates = combined
                .GroupBy(t => new { type = t["type"], id = t["id"] })
                .Where(g => g.Count() > 1)
                .Select(g => new { resource = g.Key, count = g.Count() })
                .ToList();

            Assert.Equal(0, duplicates.Count);
        }

        [Fact(DisplayName = "Serializes collections as such")]
        public virtual void SerializesCollectionsAsSuch()
        {
            var people = new[] { new Person(true, "123") };

            var target = GetSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();

            Assert.True(result["data"] is JArray);
            Assert.True((result["data"] as JArray)[0]["id"].Value<string>() == "123");
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetQuery(string key, string value)
        {
            yield return new KeyValuePair<string, string>(key, value);
        }

        internal static Uri GetUri(string path = "/people/", string id = null, string query = null)
        {
            var combined = "/api" + path;
            if (id != null) combined += id;
            if (query != null) combined += "?" + query;

            return new Uri(DefaultUrl, combined);
        }
    }
}