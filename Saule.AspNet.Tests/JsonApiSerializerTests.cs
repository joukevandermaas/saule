using System;
using System.IO;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Serialization;
using Tests.Helpers;
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

        [Fact(DisplayName = "Uses query filter expressions if specified")]
        public void UsesQueryFilterExpressions()
        {
            var target = new JsonApiSerializer<CompanyResource>
            {
                AllowQuery = true
            };
            target.QueryFilterExpressions.SetExpression<LocationType>((left, right) => left != right);

            var companies = Get.Companies(100).ToList().AsQueryable();
            var result = target.Serialize(companies, new Uri(DefaultUrl, "?filter[location]=1"));
            _output.WriteLine(result.ToString());

            var filtered = ((JArray)result["data"]).ToList();

            var expected = companies.Where(x => x.Location != LocationType.National).ToList();

            Assert.Equal(expected.Count, filtered.Count);
        }

        [Fact(DisplayName = "Applies filtering if allowed")]
        public void AppliesFilters()
        {
            var target = new JsonApiSerializer<CompanyResource>
            {
                AllowQuery = true
            };

            var companies = Get.Companies(100).ToList().AsQueryable();
            var result = target.Serialize(companies, new Uri(DefaultUrl, "?filter[location]=1"));
            _output.WriteLine(result.ToString());

            var filtered = ((JArray)result["data"]).ToList();

            var expected = companies.Where(x => x.Location == LocationType.National).ToList();

            Assert.Equal(expected.Count, filtered.Count);
        }

        [Fact(DisplayName = "Does not apply filtering if not allowed")]
        public void ConditionallyAppliesFilters()
        {

        }

        [Fact(DisplayName = "Does not allow null Uri")]
        public void HasAContract()
        {
            var target = new JsonApiSerializer<PersonResource>();
            Assert.Throws<ArgumentNullException>(() => target.Serialize(Get.Person(), null));
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
            var people = Get.People(20).AsQueryable();
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
            var people = Get.People(5);
            var result = target.Serialize(people, DefaultUrl);

            _output.WriteLine(result.ToString());

            Assert.Equal(5, result["data"].Count());
            Assert.Null(result["links"]["next"]);
            Assert.Null(result["links"]["first"]);
            Assert.NotNull(result["links"]["self"]);
        }

        [Fact(DisplayName = "Applies sorting when allowed by property")]
        public void AppliesSorting()
        {
            var target = new JsonApiSerializer<PersonResource>
            {
                AllowQuery = true
            };

            // people needs to be > 80 so we always get doubles and we can 
            // verify the -id properly
            var people = Get.People(100).AsQueryable();
            var result = target.Serialize(people, new Uri(DefaultUrl, "?sort=+age,-id"));
            _output.WriteLine(result.ToString());

            var props = ((JArray)result["data"]).Select(t => new
            {
                Age = t["attributes"]["age"].Value<int>(),
                Id = t["id"].Value<string>()
            }).ToList();

            var expected = props.OrderBy(p => p.Age).ThenByDescending(p => p.Id);

            Assert.Equal(expected, props);
        }

        [Fact(DisplayName = "Applies including when includes specified")]
        public void AppliesIncluding()
        {
            var target = new JsonApiSerializer<PersonResource>()
            {
                AllowQuery = true
            };
            var people = Get.People(2).AsQueryable();

            var result = target.Serialize(people, new Uri(DefaultUrl, "?include=job,car"));
            _output.WriteLine(result.ToString());

            var included = ((JArray)result["included"])
                .Where(x => 
                    x["type"].Value<string>() == "corporation" ||
                    x["type"].Value<string>() == "car")
                .ToList();

            Assert.Equal(2, included.Count);
        }

        [Fact(DisplayName = "Uses converters")]
        public void UsesConverters()
        {
            var target = new JsonApiSerializer<CompanyResource>();
            var company = Get.Company();
            target.JsonConverters.Add(new StringEnumConverter());
            var result = target.Serialize(company, DefaultUrl);

            _output.WriteLine(result.ToString());

            Assert.Equal(company.Location.ToString(), result["data"]["attributes"].Value<string>("location"));
        }

        [Fact(DisplayName = "Uses UrlPathBuilder")]
        public void UsesUrlBuilder()
        {
            var target = new JsonApiSerializer<PersonResource>
            {
                UrlPathBuilder = new CanonicalUrlPathBuilder()
            };

            var result = target.Serialize(Get.Person(), DefaultUrl);
            _output.WriteLine(result.ToString());

            var related = result["data"]["relationships"]["job"]["links"]["related"].Value<Uri>()
                .AbsolutePath;

            Assert.Equal("/corporations/456/", related);
        }

        [Fact(DisplayName = "Applies sorting before pagination")]
        public void QueryOrderCorrect()
        {
            var target = new JsonApiSerializer<PersonResource>
            {
                AllowQuery = true,
                Paginate = true,
                ItemsPerPage = 10
            };

            var people = Get.People(100).AsQueryable();
            var result = target.Serialize(people, new Uri(DefaultUrl, "?sort=-id"));
            _output.WriteLine(result.ToString());

            var ids = ((JArray)result["data"]).Select(t => t["id"].Value<string>());
            var expected = Enumerable.Range(0, 100)
                .OrderByDescending(i => i)
                .Take(10)
                .Select(i => i.ToString());

            Assert.Equal(expected, ids);
        }

        [Fact(DisplayName = "Gives useful error when sorting on non-existing property (queryable)")]
        public void GivesUsefulErrorForQueryable()
        {
            var target = new JsonApiSerializer<PersonResource>
            {
                AllowQuery = true
            };

            var people = Get.People(100).AsQueryable();
            var result = target.Serialize(people, new Uri(DefaultUrl, "?sort=fail-me"));
            _output.WriteLine(result.ToString());

            var error = result["errors"][0];

            Assert.Equal("Attribute 'fail-me' not found.", error["title"].Value<string>());
        }

        [Fact(DisplayName = "Gives useful error when sorting on non-existing property (enumerable)")]
        public void GivesUsefulErrorForEnumerable()
        {
            var target = new JsonApiSerializer<PersonResource>
            {
                AllowQuery = true
            };

            var people = Get.People(100);
            var result = target.Serialize(people, new Uri(DefaultUrl, "?sort=fail-me"));
            _output.WriteLine(result.ToString());

            var error = result["errors"][0];

            Assert.Equal("Attribute 'fail-me' not found.", error["title"].Value<string>());
        }
    }
}
