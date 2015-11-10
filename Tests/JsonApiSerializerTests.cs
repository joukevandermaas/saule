using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Converters;
using Saule;
using Tests.Helpers;
using Tests.Models;
using Xunit;

namespace Tests
{
    public class JsonApiSerializerTests
    {
        private static Uri DefaultUrl => new Uri("http://example.com/api/people");

        [Fact(DisplayName = "Does not allow null Uri")]
        public void HasAContract()
        {
            var target = new JsonApiSerializer<PersonResource>();
            Assert.Throws<ArgumentNullException>(() => target.Serialize(new Person(), null));
        }

        [Fact(DisplayName = "Serializes Exceptions as errors")]
        public void WorksOnExceptions()
        {
            var target = new JsonApiSerializer<PersonResource>();
            var result = target.Serialize(new FileNotFoundException(), DefaultUrl);

            Assert.Null(result["data"]);
            Assert.NotNull(result["errors"]);
        }

        [Fact(DisplayName = "Serializes HttpErrors as errors")]
        public void WorksOnHttpErrors()
        {
            var target = new JsonApiSerializer<PersonResource>();
            var result = target.Serialize(new HttpError(), DefaultUrl);

            Assert.Null(result["data"]);
            Assert.NotNull(result["errors"]);
        }

        [Fact(DisplayName = "Uses pagination if property set")]
        public void AppliesPagination()
        {
            var target = new JsonApiSerializer<PersonResource>
            {
                ItemsPerPage = 5,
                Paginate = true
            };
            var people = GetPeople(20).AsQueryable();
            var result = target.Serialize(people, DefaultUrl);

            Assert.Equal(5, result["data"].Count());
            Assert.NotNull(result["links"]["next"]);
            Assert.NotNull(result["links"]["first"]);
            Assert.Null(result["links"]["prev"]);
        }

        [Fact(DisplayName = "Ignores ItemsPerPage if Paginate is disabled")]
        public void PropertyInteraction()
        {
            var target = new JsonApiSerializer<PersonResource>
            {
                ItemsPerPage = 2,
                Paginate = false
            };
            var people = GetPeople(5);
            var result = target.Serialize(people, DefaultUrl);

            Assert.Equal(5, result["data"].Count());
            Assert.Null(result["links"]["next"]);
            Assert.Null(result["links"]["first"]);
            Assert.NotNull(result["links"]["self"]);
        }

        [Fact(DisplayName = "Uses converters")]
        public void UsesConverters()
        {
            var target = new JsonApiSerializer<TestConvertersResource>();
            target.JsonConverters.Add(new StringEnumConverter());
            var result = target.Serialize(new TestConverters(), DefaultUrl);

            Assert.Equal("Second", result["data"]["attributes"].Value<string>("property"));
        }

        private static IEnumerable<Person> GetPeople(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Person(prefill: true, id: (i + 1).ToString());
            }
        }

        private class TestConvertersResource : ApiResource
        {
            public TestConvertersResource()
            {
                Attribute("property");
            }
        }

        private class TestConverters
        {
            public enum Test
            {
                First,
                Second
            }

            public Test Property { get; set; } = Test.Second;
            public string Id { get; set; } = Test.First.ToString();
        }
    }
}
