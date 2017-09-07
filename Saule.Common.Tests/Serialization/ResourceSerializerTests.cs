using System;
using System.Collections.Generic;
using System.Dynamic;
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
using Moq;

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
            var thirdModel = new Recursion.ThirdModel();
            var fourthModel = new Recursion.FourthModel();
            firstModel.Child = secondModel;
            secondModel.Parent = firstModel;
            secondModel.Child = thirdModel;
            thirdModel.Parent = secondModel;
            thirdModel.Child = fourthModel;
            fourthModel.Parent = thirdModel;

            var target = new ResourceSerializer(firstModel, new Recursion.FirstModelResource(),
                GetUri(id: firstModel.Id), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());


            var included = result["included"] as JArray;

            Assert.NotNull(included);


            var secondOutput = included
                .Where(t => t["type"].Value<string>() == "second-model").FirstOrDefault();

            Assert.NotNull(secondOutput);


            var parentReference = secondOutput["relationships"]?["parent"]?["data"]?["type"];

            Assert.NotNull(parentReference);
            Assert.Equal(parentReference.Value<string>(), "first-model");


            var childReference = secondOutput["relationships"]?["child"]?["data"]?["type"];

            Assert.NotNull(childReference);
            Assert.Equal(childReference.Value<string>(), "third-model");
        }

        [Fact(DisplayName = "Uses a property called 'Id' when none is specified for Ids")]
        public void UsesDefaultPropertyId()
        {
            var data = new PersonWithNoJob();
            var target = new ResourceSerializer(data, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(data.Id, id);
        }

        [Fact(DisplayName = "Uses specified property if it exists for Ids")]
        public void UsesSpecifiedPropertyId()
        {
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "abc"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"].Value<string>("id");

            Assert.Equal(DefaultObject.Identifier, id);
        }

        [Fact(DisplayName = "Uses custom id property for urls")]
        public void UsesCustomIdInUrls()
        {
            var person = Get.Person(id: "abc");
            person.Friends = Get.People(1);
            var target = new ResourceSerializer(person, DefaultResource,
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
                GetUri(id: "abc"), DefaultPathBuilder, null, null);

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
                GetUri(id: "abc"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var id = result["data"]["relationships"]["job"]["data"].Value<string>("id");

            Assert.Equal(person.Job.CompanyId, id);
        }

        [Fact(DisplayName = "Serializes all found attributes")]
        public void AttributesComplete()
        {
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
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
        public void AttributesSufficient()
        {
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var attributes = result["data"]["attributes"];
            Assert.True(attributes["numberOfLegs"] == null, null);
            Assert.Equal(4, attributes.Count());
        }

        [Fact(DisplayName = "Uses type name from model definition")]
        public void UsesTitle()
        {
            var company = Get.Company();
            var target = new ResourceSerializer(company, new CompanyResource(),
                GetUri("/corporations", "456"),
                DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal("corporation", result["data"]["type"]);
        }

        [Fact(DisplayName = "Throws exception when Id is missing")]
        public void ThrowsRightException()
        {
            var person = new PersonWithNoId();
            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, null);

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
        public void IncludesRelationshipData()
        {
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
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
        public void NoIncludedRelationshipData()
        {
            var includes = new IncludingContext();
            includes.DisableDefaultIncluded = true;
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, includes);
            
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            Assert.Null(included);
        }

        [Fact(DisplayName = "Serialize only included relationship data into 'included' key when includedDefault set to false")]
        public void OnlyIncludedRelationshipData()
        {
            var includes = new IncludingContext();
            includes.DisableDefaultIncluded = true;
            var includeParam = new KeyValuePair<string, string>("include", "job");
            includes.SetIncludes(new List<KeyValuePair<string, string>>() { includeParam });
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, includes);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            Assert.Equal(1, included.Count());
        }

        [Fact(DisplayName = "Serialize relationship identifier objects into 'data' key when includedDefault set to true")]
        public void IncludedRelationshipIdentifierObjects()
        {
            var includes = new IncludingContext();
            includes.DisableDefaultIncluded = true;
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, includes);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];

            Assert.NotNull(job["data"]);
        }

        [Fact(DisplayName = "Omit data for relationship objects not existing as property in the original model")]
        public void OmitRelationshipIdentifierObjectsWithoutProperty()
        {
            var includes = new IncludingContext();
            includes.DisableDefaultIncluded = true;
            var target = new ResourceSerializer(DefaultObject, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, includes);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var secretFriends = relationships["secret-friends"];

            Assert.Null(secretFriends["data"]);
        }

        [Fact(DisplayName = "Relationships of included resources have correct URLs")]
        public void IncludedResourceRelationshipURLsAreCorrect()
        {
            var person = new Person(true)
            {
                Job = new CompanyWithCustomers(true)
            };

            var target = new ResourceSerializer(person, new PersonWithCompanyWithCustomersResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            Assert.Equal("http://example.com/api/corporations/456/relationships/customers/", included?[0]?["relationships"]?["customers"]?["links"]?.Value<Uri>("self")?.ToString()); 
        }

        [Fact(DisplayName = "Explicitly included resource referenced in multiple resources is only included once")]
        public void IncludedResourceOnlyOnce()
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
            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "123"), DefaultPathBuilder, null, include);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            Assert.Equal(3, included.Count);
        }

        [Fact(DisplayName = "Handles null relationships and attributes correctly")]
        public void HandlesNullValues()
        {
            var person = new Person(id: "45");
            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "45"), DefaultPathBuilder, null, new IncludingContext { DisableDefaultIncluded = true });

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var attributes = result["data"]["attributes"];

            Assert.NotNull(attributes["first-name"]);
            Assert.NotNull(attributes["last-name"]);
            Assert.NotNull(attributes["age"]);

            Assert.Equal(0, relationships["job"]["data"].Count());
            Assert.Equal(0, relationships["friends"]["data"].Count());
        }

        [Fact(DisplayName = "Serializes enumerables properly")]
        public void SerializesEnumerables()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;
            var jobLinks = (result["data"] as JArray)?[0]["relationships"]["job"]["links"];

            Assert.Equal(2, included?.Count);
            Assert.Equal("/api/people/0/employer/", jobLinks?.Value<Uri>("related").AbsolutePath);
        }

        [Fact(DisplayName = "Document MUST contain at least one: data, errors, meta")]
        public void DocumentMustContainAtLeastOneDataOrErrorOrMeta()
        {
            var people = new Person[] { };
            var target = new ResourceSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.NotNull(result["data"]);
        }

        [Fact(DisplayName = "If a document does not contain a top-level data key, the included member MUST NOT be present either.")]
        public void DocumentMustNotContainIncludedForEmptySet()
        {
            var people = new Person[0];
            var target = new ResourceSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["included"]);
        }

        [Fact(DisplayName = "Handles null objects correctly")]
        public void HandlesNullResources()
        {
            var target = new ResourceSerializer(null, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(JTokenType.Null, result["data"].Type);
        }

        [Fact(DisplayName = "Supports Guids for ids")]
        public void SupportsGuidIds()
        {
            var guid = new GuidAsId();
            var serializer = new ResourceSerializer(guid, new PersonWithDefaultIdResource(), 
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var guidResult = serializer.Serialize();
            _output.WriteLine(guidResult.ToString());

            Assert.NotNull(guidResult["data"]["id"]);
            Assert.Equal(JToken.FromObject(guid.Id), guidResult["data"]["id"]);
        }

        [Fact(DisplayName = "Supports Guids for ids in collections")]
        public void SupportsGuidIdsInCollections()
        {
            var guids = new [] { new GuidAsId(), new GuidAsId() };
            var serializer = new ResourceSerializer(guids, new PersonWithDefaultIdResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var guidsResult = serializer.Serialize();
            _output.WriteLine(guidsResult.ToString());

            Assert.NotEmpty(guidsResult["data"]);
        }

        [Fact(DisplayName = "Supports Guids for id in relations")]
        public void SupportsGuidsAsRelations()
        {
            var relatedToGuidId = new GuidAsRelation();
            var serializer = new ResourceSerializer( relatedToGuidId, new PersonWithGuidAsRelationsResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);

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
                GetUri(id: "123"), DefaultPathBuilder, null, null);

            var result = serializer.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["data"]["attributes"]["name"]);
            Assert.Null(result["data"]["attributes"]["location"]);
            Assert.Null(result["data"]["attributes"]["number-of-employees"]);
        }

        [Fact(DisplayName = "A compound document MUST NOT include more than one resource object for each type and id pair")]
        public void ResourceObjectsAreNotDuplicated()
        {
            var personA = new Person(false, "1");
            var personB = new Person(false, "2");
            personA.Friends = new Person[] { personB };
            personB.Friends = new Person[] { personA };

            var people = new Person[] { personA, personB };

            var target = new ResourceSerializer(people, DefaultResource,
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
        public void SerializesCollectionsAsSuch()
        {
            var people = new[] { new Person(true, "123") };

            var target = new ResourceSerializer(people, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();

            Assert.True(result["data"] is JArray);
            Assert.True((result["data"] as JArray)[0]["id"].Value<string>() == "123");
        }

        [Fact(DisplayName = "Included resources have correct relationship linkage")]
        public void IncludedResourcesHaveCorrectRelationshipLinkage()
        {
            var personA = new Person(false, "1");
            var personB = new Person(false, "2");
            var personC = new Person(false, "3");
            personA.Friends = new Person[] { personB };
            personB.Friends = new Person[] { personA, personC };
            personC.Friends = new Person[] { personB };

            var somePeople = new Person[] { personA, personB };

            var target = new ResourceSerializer(somePeople, DefaultResource,
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var included = result["included"] as JArray;

            var serialisedPersonB = included
                .Where(i => i["id"].Value<string>() == "3")
                .First();

            var areFriends = serialisedPersonB["relationships"]["friends"]["data"]
                .Any(d => d["id"].Value<string>() == "2");

            Assert.True(areFriends);
        }


        [Fact(DisplayName = "All Ids are serialized as strings")]
        public void AllIdsAreSerializedAsStrings()
        {
            var w = new Widget
            {
                Id = 1,
                Title = "Title"
            };

            var target = new ResourceSerializer(w, new WidgetResource(),
                GetUri(), DefaultPathBuilder, null, null);
            var result = target.Serialize();

            Assert.True(result["data"]?["id"]?.Type == JTokenType.String);
        }

        [Fact(DisplayName = "Serializes dictionaries")]
        public void SerializesDictionaries()
        {
            var person = new Dictionary<string, object>
            {
                ["Identifier"] = 1,
                ["FirstName"] = "John",
                ["LastName"] = "Smith",
                ["Age"] = 34,
                ["NumberOfLegs"] = 4
            };

            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "1"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(result["data"]["id"], "1");
            Assert.Equal(result["data"]["attributes"]["first-name"], "John");
            Assert.Equal(result["data"]["attributes"]["age"], 34);
        }

        [Fact(DisplayName = "Serializes dynamics")]
        public void SerializesDynamics()
        {
            dynamic person = new ExpandoObject();
            person.Identifier = 1;
            person.FirstName = "John";
            person.LastName = "Smith";
            person.Age = 34;
            person.NumberOfLegs = 4;

            var target = new ResourceSerializer(person, DefaultResource,
                GetUri(id: "1"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(result["data"]["id"], "1");
            Assert.Equal(result["data"]["attributes"]["first-name"], "John");
            Assert.Equal(result["data"]["attributes"]["age"], 34);
        }

        [Fact(DisplayName = "Only serializes attributes in the resource")]
        public void OnlySerializesAttributesInTheResource()
        {
            var personMock = new Mock<LazyPerson>();
            personMock.SetupGet(p => p.Identifier).Returns("123");

            var target = new ResourceSerializer(personMock.Object, DefaultResource,
                GetUri(id: "1"), DefaultPathBuilder, null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            personMock.VerifyGet(p => p.Identifier);
            Assert.Throws<MockException>(() => 
            {
                personMock.VerifyGet(p => p.NumberOfLegs);
            });
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
