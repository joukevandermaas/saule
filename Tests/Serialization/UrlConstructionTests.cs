using System;
using System.Collections.Generic;
using System.Linq;
using Saule;
using Saule.Queries.Pagination;
using Saule.Serialization;
using Tests.Helpers;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class UrlConstructionTests
    {
        private readonly ITestOutputHelper _output;

        private static Uri DefaultUrl => new Uri("http://example.com/");
        private static IUrlPathBuilder DefaultPathBuilder => new DefaultUrlPathBuilder("/api");
        private static IUrlPathBuilder CustomPathBuilder => new DefaultUrlPathBuilder("/api/location/123");

        public UrlConstructionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Handles query parameters correctly single resource")]
        public void HandlesQueryParams()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                GetUri("123", "a=b&c=d"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var jobLinks = result["data"]?["relationships"]?["job"]?["links"];

            var selfLink = result["links"].Value<string>("self");
            var jobSelfLink = jobLinks?.Value<Uri>("self")?.PathAndQuery;
            var jobRelationLink = jobLinks?.Value<Uri>("related")?.PathAndQuery;

            Assert.EndsWith("/api/people/123?a=b&c=d", selfLink);
            Assert.Equal("/api/people/123/relationships/employer/", jobSelfLink);
            Assert.Equal("/api/people/123/employer/", jobRelationLink);
        }

        [Fact(DisplayName = "Handles query parameters correctly collection resource")]
        public void HandlesQueryParamsCollection()
        {
            var target = new ResourceSerializer(Get.People(1), new PersonResource(),
                GetUri(null, "a=b&c=d"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var jobLinks = result["data"][0]?["relationships"]?["job"]?["links"];

            var selfLink = result["links"].Value<string>("self");
            var jobSelfLink = jobLinks?.Value<Uri>("self")?.PathAndQuery;
            var jobRelationLink = jobLinks?.Value<Uri>("related")?.PathAndQuery;

            Assert.EndsWith("/api/people?a=b&c=d", selfLink);
            Assert.Equal("/api/people/0/relationships/employer/", jobSelfLink);
            Assert.Equal("/api/people/0/employer/", jobRelationLink);
        }

        [Fact(DisplayName = "Items have self links in a collection")]
        public void SelfLinksInCollection()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, new PersonResource(),
                GetUri(), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            foreach (var elem in result["data"])
            {
                var links = elem["links"];
                Assert.NotNull(links);
                Assert.Equal("/api/people/" + elem.Value<string>("id") + "/", links.Value<Uri>("self").AbsolutePath);
            }
        }

        [Fact(DisplayName = "Self Link Single Resource Casing")]
        public void SelfLinkSingleResourceCasing()
        {
            var person = new PersonWithDifferentId(false, "Allen");

            var lowerTarget = new ResourceSerializer(person, new PersonWithDifferentIdResource(),
                 GetUri("allen"), DefaultPathBuilder, null, null, null);
            var upperTarget = new ResourceSerializer(person, new PersonWithDifferentIdResource(),
                GetUri("Allen"), DefaultPathBuilder, null, null, null);

            var lowerResult = lowerTarget.Serialize();
            _output.WriteLine(lowerResult.ToString());
            var upperResult = upperTarget.Serialize();
            _output.WriteLine(upperResult.ToString());

            var selfLinkLower = lowerResult["links"].Value<string>("self");
            var selfLinkUpper = upperResult["links"].Value<string>("self");

            Assert.EndsWith("/api/people/allen", selfLinkLower);
            Assert.EndsWith("/api/people/Allen", selfLinkUpper);
        }

        [Fact(DisplayName = "Items have self links in a collection with custom route and top self link")]
        public void SelfLinksInCollectionCustomRoute()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, new PersonResource(),
                 new Uri("http://localhost:80/api/location/123/people"), CustomPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var selfLink = result["links"].Value<string>("self");

            foreach (var elem in result["data"])
            {
                var links = elem["links"];
                Assert.NotNull(links);
                Assert.Equal("/api/location/123/people/" + elem.Value<string>("id") + "/", links.Value<Uri>("self").AbsolutePath);
            }

            Assert.EndsWith("/api/location/123/people", selfLink);
        }

        [Fact(DisplayName = "Item does not have self link in single element")]
        public void NoSelfLinksInObject()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var links = result["data"]?["links"];

            Assert.Null(links);
        }

        [Fact(DisplayName = "Adds top level self link")]
        public void SelfLink()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var selfLink = result["links"].Value<string>("self");

            Assert.EndsWith("/api/people/123", selfLink);
        }

        [Fact(DisplayName = "Adds top level self link without any port")]
        public void SelfLinkNoPort()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                new Uri("http://localhost:80/api/people/123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var selfLink = result["links"].Value<string>("self");

            Assert.Equal("http://localhost/api/people/123", selfLink);
        }

        [Fact(DisplayName = "Adds top level self link without any port for https")]
        public void SelfLinkNoPortHttps()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                new Uri("https://localhost:443/api/people/123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var selfLink = result["links"].Value<string>("self");

            Assert.Equal("https://localhost/api/people/123", selfLink);
        }

        [Fact(DisplayName = "Adds top level self link without any port and with correct encoding")]
        public void SelfLinkNoEncoding()
        {
            var target = new ResourceSerializer(Get.People(1), new PersonResource(),
                new Uri("https://localhost:443/api/people?page[number]=1&page[size]=10"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var selfLink = result["links"].Value<string>("self");

            Assert.Equal("https://localhost/api/people?page[number]=1&page[size]=10", selfLink);
        }

        [Fact(DisplayName = "Adds top level self link without any port and with correct encoding based on already encoded value")]
        public void SelfLinkNoEncodingBasedOnEncoded()
        {
            var target = new ResourceSerializer(Get.People(1), new PersonResource(),
                new Uri("https://localhost:443/api/people?page%5Bnumber%5D=1&page%5Bsize%5D=10"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var selfLink = result["links"].Value<string>("self");

            Assert.Equal("https://localhost/api/people?page[number]=1&page[size]=10", selfLink);
        }

        [Fact(DisplayName = "Adds top level self link if only LinkType.TopSelf is specified")]
        public void TopLinkOnlyLink()
        {
            var target = new ResourceSerializer(Get.People(1), new PersonOnlyTopLinksResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var selfLink = result["links"].Value<string>("self");

            Assert.EndsWith("/api/people/123", selfLink);

            Assert.Null(result["data"][0]["links"]);
        }

        [Fact(DisplayName = "Omits top level links if so requested")]
        public void NoTopLevelLinks()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonNoLinksResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Null(result["links"]);
        }

        [Fact(DisplayName = "Adds next link only if needed")]
        public void NextLink()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, new PersonResource(),
                GetUri(), DefaultPathBuilder,
                new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "2"), pageSizeDefault: 10), null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(null, result["links"]["next"]);

            target = new ResourceSerializer(people, new PersonResource(),
                GetUri(), DefaultPathBuilder,
                new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "2"), pageSizeDefault: 4), null, null);
            result = target.Serialize();

            var nextLink = Uri.UnescapeDataString(result["links"].Value<Uri>("next").Query);
            Assert.Equal("?page[number]=3", nextLink);
        }

        [Fact(DisplayName = "Adds previous link only if needed")]
        public void PreviousLink()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, new PersonResource(),
                GetUri(), DefaultPathBuilder,
                new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "0"), pageSizeDefault: 10), null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(null, result["links"]["prev"]);

            target = new ResourceSerializer(people, new PersonResource(),
                GetUri(), DefaultPathBuilder,
                new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "1"), pageSizeDefault: 10), null, null);
            result = target.Serialize();

            var nextLink = Uri.UnescapeDataString(result["links"].Value<Uri>("prev").Query);
            Assert.Equal("?page[number]=0", nextLink);
        }

        [Fact(DisplayName = "Adds previous link with shifted first page number and only if needed")]
        public void PreviousLinkWithShiftedFirstPage()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, new PersonResource(),
                GetUri(), DefaultPathBuilder,
                new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "1"), pageSizeDefault: 10, pageSizeLimit:null, firstPageNumber:1), null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            Assert.Equal(null, result["links"]["prev"]);

            target = new ResourceSerializer(people, new PersonResource(),
                GetUri(), DefaultPathBuilder,
                new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "2"), pageSizeDefault: 10, pageSizeLimit:null, firstPageNumber: 1), null, null);
            result = target.Serialize();

            var nextLink = Uri.UnescapeDataString(result["links"].Value<Uri>("prev").Query);
            Assert.Equal("?page[number]=1", nextLink);
        }


        [Fact(DisplayName = "Keeps other query parameters when paginating")]
        public void PaginationQueryParams()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, new PersonResource(),
                GetUri(query: "q=a"), DefaultPathBuilder,
                new PaginationContext(GetQuery("q", "a"), pageSizeDefault: 4), null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var nextLink = Uri.UnescapeDataString(result["links"].Value<Uri>("next").Query);
            Assert.Equal("?q=a&page[number]=1", nextLink);
        }

        [Fact(DisplayName = "Adds first link if paginating")]
        public void FirstLink()
        {
            var people = Get.People(5);
            var target = new ResourceSerializer(people, new PersonResource(),
               GetUri(), DefaultPathBuilder,
                new PaginationContext(Enumerable.Empty<KeyValuePair<string, string>>(), pageSizeDefault: 4), null, null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var nextLink = Uri.UnescapeDataString(result["links"].Value<Uri>("first").Query);
            Assert.Equal("?page[number]=0", nextLink);
        }

        [Fact(DisplayName = "Serializes relationships' links")]
        public void SerializesRelationshipLinks()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Equal("/api/people/123/employer/", job["links"].Value<Uri>("related").AbsolutePath);
            Assert.Equal("/api/people/123/relationships/employer/", job["links"].Value<Uri>("self").AbsolutePath);

            Assert.Equal("/api/people/123/friends/", friends["links"].Value<Uri>("related").AbsolutePath);
            Assert.Equal("/api/people/123/relationships/friends/", friends["links"].Value<Uri>("self").AbsolutePath);
        }

        [Fact(DisplayName = "Supports multiple url builders")]
        public void SerializeDifferentBuilder()
        {
            var person = Get.Person();
            person.Friends = Get.People(1);
            var target = new ResourceSerializer(person, new PersonResource(),
                GetUri("123"), new CanonicalUrlPathBuilder(), null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Equal("/corporations/456/", job["links"].Value<Uri>("related").AbsolutePath);
            Assert.Equal("/people/123/relationships/employer/", job["links"].Value<Uri>("self").AbsolutePath);

            Assert.Equal("/people/123/relationships/friends/", friends["links"].Value<Uri>("self").AbsolutePath);
            Assert.Null(friends["links"]["relationships"]);
        }

        [Fact(DisplayName = "Builds absolute links correctly")]
        public void BuildsRightLinks()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var job = result["data"]["relationships"]["job"];

            Assert.Equal("http://example.com/api/people/123/employer/",
                job["links"].Value<Uri>("related").ToString());
            Assert.Equal("http://example.com/api/people/123/relationships/employer/",
                job["links"].Value<Uri>("self").ToString());
        }

        [Fact(DisplayName = "Omits relationship links if so requested")]
        public void NoRelLinks()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonNoJobLinksResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var job = result["data"]["relationships"]["job"];

            Assert.Null(job["links"]);
        }

        [Fact(DisplayName = "Omits relationship related links if so requested")]
        public void NoRelRelLinks()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonJobOnlySelfLinksResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var job = result["data"]["relationships"]["job"];

            Assert.Null(job["links"]["related"]);
            Assert.Equal("http://example.com/api/person-job-only-self-links/123/relationships/employer/",
                job["links"].Value<Uri>("self").ToString());
        }

        [Fact(DisplayName = "Omits relationship self links if so requested")]
        public void NoRelSelfLinks()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonJobOnlyRelatedLinksResource(),
                GetUri("123"), DefaultPathBuilder, null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var job = result["data"]["relationships"]["job"];

            Assert.Null(job["links"]["self"]);
            Assert.Equal("http://example.com/api/person-job-only-related-links/123/employer/",
                job["links"].Value<Uri>("related").ToString());
        }

        [Fact(DisplayName = "Does not generate links when url builder returns nothing")]
        public void UrlBuilder()
        {
            var target = new ResourceSerializer(Get.Person(), new PersonResource(),
                GetUri("123"), new EmptyUrlBuilder(), null, null, null);
            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var job = result["data"]["relationships"]["job"];

            Assert.Equal(null, job["links"]);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQuery(string key, string value)
        {
            yield return new KeyValuePair<string, string>(key, value);
        }

        private static Uri GetUri(string id = null, string query = null)
        {
            var path = "/api/people";
            if (id != null) path += $"/{id}";
            if (query != null) path += "?" + query;

            return new Uri(DefaultUrl, path);
        }

        private class EmptyUrlBuilder : IUrlPathBuilder
        {
            public string BuildCanonicalPath(ApiResource resource)
            {
                return string.Empty;
            }

            public string BuildCanonicalPath(ApiResource resource, string id)
            {
                return string.Empty;
            }

            public string BuildRelationshipPath(ApiResource resource, string id, ResourceRelationship relationship)
            {
                return string.Empty;
            }

            public string BuildRelationshipPath(ApiResource resource, string id, ResourceRelationship relationship,
                string relatedResourceId)
            {
                return string.Empty;
            }
        }
    }
}
