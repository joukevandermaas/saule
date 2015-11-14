using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Converters;
using Saule;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class JsonApiSerializerTests
    {
        private readonly ITestOutputHelper _output;
        private static Uri DefaultUrl => new Uri("http://example.com/api/people");

        public JsonApiSerializerTests(ITestOutputHelper output)
        {
            _output = output;
        }

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

            _output.WriteLine(result.ToString());

            Assert.Null(result["data"]);
            Assert.NotNull(result["errors"]);
        }

        [Fact(DisplayName = "Serializes HttpErrors as errors")]
        public void WorksOnHttpErrors()
        {
            var target = new JsonApiSerializer<PersonResource>();
            var result = target.Serialize(new HttpError(), DefaultUrl);

            _output.WriteLine(result.ToString());

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

            _output.WriteLine(result.ToString());

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

            _output.WriteLine(result.ToString());

            Assert.Equal(5, result["data"].Count());
            Assert.Null(result["links"]["next"]);
            Assert.Null(result["links"]["first"]);
            Assert.NotNull(result["links"]["self"]);
        }

        [Fact(DisplayName = "Uses converters")]
        public void UsesConverters()
        {
            var target = new JsonApiSerializer<CompanyResource>();
            target.JsonConverters.Add(new StringEnumConverter());
            var result = target.Serialize(new Company(prefill: true), DefaultUrl);

            _output.WriteLine(result.ToString());

            Assert.Equal("National", result["data"]["attributes"].Value<string>("location"));
        }

        private static IEnumerable<Person> GetPeople(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Person(prefill: true, id: (i + 1).ToString());
            }
        }
    }
}
