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
    [Trait("Serializer", "ResourceGraphSerializer")]
    public class ResourceGraphSerializerTests : ResourceSerializerTests
    {
        public ResourceGraphSerializerTests(ITestOutputHelper output)
            : base(output)
        {
        }

        internal override IResourceSerializer GetSerializer(
            object value,
            ApiResource type,
            Uri baseUrl,
            IUrlPathBuilder urlBuilder,
            PaginationContext paginationContext,
            IncludingContext includingContext)
        {
            _output.WriteLine("Serializer type: ResourceGraphSerializer");
            return new ResourceGraphSerializer(value, type, baseUrl, urlBuilder, paginationContext, includingContext);
        }

        public override void IncludedResourceOnlyOnce()
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

            Assert.Equal(3, included.Count);
            /**
             *  Changed from Assert.Equal(4, included.Count) in the original version
             *  - an includes query of "friends.job" should only include the job of 
             *    the person's friends, not the person's job
             */
        }

        public override void SerializesRelationshipData()
        {
            var person = new PersonWithNoJob();
            var target = GetSerializer(person, new PersonWithDefaultIdResource(),
                GetUri(id: "123"), DefaultPathBuilder, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Equal(job["data"].Type.ToString(), "Null");
            /**
             *  Changed from Assert.Null(job["data"]) in the original version
             *  - relationships that are known to be null or empty should return data representing that
             *    an absence of the data property indicates that a client should request the link to 
             *    expand the relationship
             */

            Assert.NotNull(friends);
        }

        public override void HandlesNullValues()
        {
            var person = new Person(id: "45");
            var target = GetSerializer(person, DefaultResource,
                GetUri(id: "45"), DefaultPathBuilder, null, new IncludingContext { DisableDefaultIncluded = true });
            /**
             *  Changed from a null IncludingContext in the original version
             *  - relationships that are known to be null or empty should return data representing that
             *    an absence of the data property indicates that a client should request the link to 
             *    expand the relationship
             */

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

        [Fact(DisplayName = "Included resources have correct relationship linkage")]
        public virtual void IncludedResourcesHaveCorrectRelationshipLinkage()
        {
            /**
             * This test fails for the original serializer implementation
             */
            var personA = new Person(false, "1");
            var personB = new Person(false, "2");
            var personC = new Person(false, "3");
            personA.Friends = new Person[] { personB };
            personB.Friends = new Person[] { personA, personC };
            personC.Friends = new Person[] { personB };

            var somePeople = new Person[] { personA, personB };

            var target = GetSerializer(somePeople, DefaultResource,
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
        
        public override void HandlesRecursiveProperties()
        {
            /**
             * The overidden test does not test for relationships
             * This test fails for the original serializer implementation
             */

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
    }
}