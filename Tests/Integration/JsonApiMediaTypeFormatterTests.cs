using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Saule.Http;
using Saule.Serialization;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Integration
{
    public class JsonApiMediaTypeFormatterTests
    {
        private readonly ITestOutputHelper _output;

        public JsonApiMediaTypeFormatterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public class ObsoleteSetup
        {
            private readonly ITestOutputHelper _output;

            public ObsoleteSetup(ITestOutputHelper output)
            {
                _output = output;
            }

            [Fact(DisplayName = "Default constructor uses DefaultUrlPathBuilder and no converters")]
            public async Task DefaultConstructorObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter();

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/companies/456/");
                    _output.WriteLine(result.ToString());

                    Assert.Equal(1, result["data"]["attributes"]["location"].Value<int>());

                    result = await client.GetJsonResponseAsync("api/people/");
                    _output.WriteLine(result.ToString());

                    var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                        .Value<string>();
                    Assert.Equal("http://localhost/api/people/0/employer/", relatedUrl);
                }
            }

            [Fact(DisplayName = "Url builder constructor generates those urls")]
            public async Task UrlBuilderConstructorObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter(
                    new CanonicalUrlPathBuilder());

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/people/");
                    _output.WriteLine(result.ToString());

                    var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                        .Value<string>();
                    Assert.Equal("http://localhost/corporations/456/", relatedUrl);
                }
            }

            [Fact(DisplayName = "Converter constructor uses that converter")]
            public async Task ConverterConstructorObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter(
                    new StringEnumConverter());

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/companies/456/");
                    _output.WriteLine(result.ToString());

                    Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());
                }
            }

            [Fact(DisplayName = "Builder and converter constructor uses both of those")]
            public async Task BuilderAndConverterConstructorObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter(
                    new CanonicalUrlPathBuilder(),
                    new StringEnumConverter());

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/companies/456/");
                    _output.WriteLine(result.ToString());

                    Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());

                    result = await client.GetJsonResponseAsync("api/people/");
                    _output.WriteLine(result.ToString());

                    var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                        .Value<string>();
                    Assert.Equal("http://localhost/corporations/456/", relatedUrl);
                }
            }

            [Fact(DisplayName = "Applies pagination when appropriate")]
            public async Task AppliesPaginationObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter();

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/companies/");
                    _output.WriteLine(result.ToString());

                    Assert.Equal(12, (result["data"] as JArray)?.Count);
                }
            }

            [Fact(DisplayName = "Applies sorting when appropriate")]
            public async Task AppliesSortingObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter();

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/query/people?sort=age");
                    _output.WriteLine(result.ToString());

                    var ages = ((JArray)result["data"])
                        .Select(p => p["attributes"]["age"].Value<int>())
                        .ToList();
                    var sorted = ages.OrderBy(a => a).ToList();

                    Assert.Equal(sorted, ages);
                }
            }

            [Fact(DisplayName = "Applies filtering when appropriate")]
            public async Task AppliesFilteringObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter();

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/query/people?filter[last-name]=Russel");
                    _output.WriteLine(result.ToString());

                    var names = ((JArray)result["data"])
                        .Select(p => p["attributes"]["last-name"].Value<string>())
                        .ToList();

                    var filtered = names.Where(a => a == "Russel").ToList();

                    Assert.Equal(filtered.Count, names.Count);
                }
            }

            [Fact(DisplayName = "Does not apply sorting when not allowed")]
            public async Task AppliesSortingConditionallyObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter();

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/people?sort=age");
                    _output.WriteLine(result.ToString());

                    var ages = ((JArray)result["data"])
                        .Select(p => p["attributes"]["age"].Value<int>())
                        .ToList();
                    var sorted = ages.OrderBy(a => a).ToList();

                    Assert.NotEqual(sorted, ages);
                }
            }

            [Theory(DisplayName = "Always does sorting before pagination")]
            [InlineData("query/paginate")]
            [InlineData("paginate/query")]
            public async Task AppliesSortingBeforePaginationObsolete(string path)
            {
                var formatter = new JsonApiMediaTypeFormatter();

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result1 = await client.GetJsonResponseAsync($"api/{path}/people?sort=age");
                    var result2 = await client.GetJsonResponseAsync($"api/{path}/people?sort=age&page[number]=1");
                    _output.WriteLine(result1.ToString());
                    _output.WriteLine(result2.ToString());

                    var ages1 = ((JArray)result1["data"])
                        .Select(p => p["attributes"]["age"].Value<int>())
                        .ToList();
                    var ages2 = ((JArray)result2["data"])
                        .Select(p => p["attributes"]["age"].Value<int>())
                        .ToList();

                    var sorted = ages1.Concat(ages2).OrderBy(a => a).ToList();

                    Assert.Equal(sorted, ages1.Concat(ages2).ToList());
                }
            }

            [Fact(DisplayName = "Gives useful error when you don't add ReturnsResourceAttribute")]
            public async Task GivesUsefulErrorObsolete()
            {
                var formatter = new JsonApiMediaTypeFormatter();

                using (var server = new ObsoleteSetupJsonApiServer(formatter))
                {
                    var client = server.GetClient();
                    var result = await client.GetJsonResponseAsync("api/broken/123/");
                    _output.WriteLine(result.ToString());

                    var error = result["errors"][0];

                    Assert.Equal("https://github.com/joukevandermaas/saule/wiki",
                        error["links"]["about"].Value<string>());

                    Assert.Equal("Saule.JsonApiException",
                        error["code"].Value<string>());

                    Assert.Equal("Saule.JsonApiException: You must add a [ReturnsResourceAttribute] to action methods.",
                        error["detail"].Value<string>());
                }
            }
        }

        [Fact(DisplayName = "Default constructor finds correct path namespace and uses no converters")]
        public async Task DefaultConstructor()
        {
            using (var server = new NewSetupJsonApiServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal(1, result["data"]["attributes"]["location"].Value<int>());

                result = await client.GetJsonResponseAsync("api/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/api/people/0/employer/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Url builder constructor generates those urls")]
        public async Task UrlBuilderConstructor()
        {
            var config = new JsonApiConfiguration
            {
                UrlPathBuilder = new CanonicalUrlPathBuilder()
            };

            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/corporations/456/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Converter constructor uses that converter")]
        public async Task ConverterConstructor()
        {
            var config = new JsonApiConfiguration
            {
                JsonConverters = { new StringEnumConverter() }
            };

            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());
            }
        }

        [Fact(DisplayName = "Builder and converter constructor uses both of those")]
        public async Task BuilderAndConverterConstructor()
        {
            var config = new JsonApiConfiguration
            {
                UrlPathBuilder = new CanonicalUrlPathBuilder(),
                JsonConverters = { new StringEnumConverter() }
            };

            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());

                result = await client.GetJsonResponseAsync("api/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/corporations/456/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Applies pagination with fixed page size from attribute")]
        public async Task AppliesPaginationPageSizeFromAttribute()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/companies/");
                _output.WriteLine(result.ToString());

                Assert.Equal(12, (result["data"] as JArray)?.Count);
            }
        }

        [Fact(DisplayName = "Applies pagination with page size from query string")]
        public async Task AppliesPaginationPageSizeFromClient()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/companies/querypagesizelimit50/?page[size]=5");
                _output.WriteLine(result.ToString());

                var resultCount = ((JArray)result["data"])?.Count;
                Assert.Equal(5, resultCount);
            }
        }

        [Fact(DisplayName = "Limits page sizes to 1 and default page size is set to 1 too")]
        public async Task LimitsPageSize1()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();

                // maximum page size equal to client page size and it works
                var result = await client.GetFullJsonResponseAsync("api/companies/querypagesizelimit1/?page[size]=1");
                var resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(1, resultCount);

                // maximum page size is lower than client page size and it fails
                result = await client.GetFullJsonResponseAsync("api/companies/querypagesizelimit1/?page[size]=10");
                resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.Null(resultCount);

                // client page size isn't specified and it works and returns one record
                result = await client.GetFullJsonResponseAsync("api/companies/querypagesizelimit1/");
                resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(1, resultCount);
            }
        }

        [Fact(DisplayName = "Maximum page sizes is set to 50 and default page size is set to 12")]
        public async Task LimitsPageSize50()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();

                // maximum page size is not set and client page size is 1 and it works
                var result = await client.GetFullJsonResponseAsync("api/companies/querypagesizelimit50/?page[size]=1");
                var resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(1, resultCount);

                // maximum page size is not set and client page size is 1000 and it works
                result = await client.GetFullJsonResponseAsync("api/companies/querypagesizelimit50/?page[size]=100");
                resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.Null(resultCount);

                // client page size isn't specified and it works and returns 12 records 
                // as default paging for this endpoint is 12
                result = await client.GetFullJsonResponseAsync("api/companies/querypagesizelimit50/");
                resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(12, resultCount);
            }
        }

        [Fact(DisplayName = "Maximum page sizes is not set, but default page size is set to 12")]
        public async Task LimitsPageSizeIsNotSet()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();

                // maximum page size is not set and client page size is 1 and it works
                var result = await client.GetFullJsonResponseAsync("api/companies/?page[size]=1");
                var resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(1, resultCount);

                // maximum page size is not set and client page size is 1000 and it works and returns 100 records
                result = await client.GetFullJsonResponseAsync("api/companies/?page[size]=1000");
                resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(100, resultCount);

                // client page size isn't specified and it works and returns 12 records 
                // as default paging for this endpoint is 12
                result = await client.GetFullJsonResponseAsync("api/companies/");
                resultCount = ((JArray)result.Content["data"])?.Count;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(12, resultCount);
            }
        }

        [Fact(DisplayName = "Paged result calculates page counts")]
        public async Task PagedResult()
        {
            await PagedResultInternal("api/companies/paged-result", 0);
        }

        [Fact(DisplayName = "Paged result calculates page counts and correct first page number")]
        public async Task PagedResultWithFirstPage()
        {
            await PagedResultInternal("api/companies/paged-result-first-page", 1);
        }

        private async Task PagedResultInternal(string baseUrl, int firstPageNumber)
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                // endpoint will return totalCount 100 items so we can calculate page numbers based on it

                // validate 100 pages by 1 page size
                var result = await client.GetFullJsonResponseAsync($"{baseUrl}?page[size]=1");
                var resultCount = ((JArray)result.Content["data"])?.Count;
                var last = result.Content["links"]["last"].Value<string>();
                var first = result.Content["links"]["first"].Value<string>();
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(1, resultCount);
                Assert.EndsWith($"{baseUrl}?page[size]=1&page[number]=100", last);
                Assert.EndsWith($"{baseUrl}?page[size]=1&page[number]={firstPageNumber}", first);

                // 12 pages by 9 page size
                result = await client.GetFullJsonResponseAsync($"{baseUrl}?page[size]=9");
                resultCount = ((JArray)result.Content["data"])?.Count;
                last = result.Content["links"]["last"].Value<string>();
                first = result.Content["links"]["first"].Value<string>();
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(9, resultCount);
                Assert.EndsWith($"{baseUrl}?page[size]=9&page[number]=12", last);
                Assert.EndsWith($"{baseUrl}?page[size]=9&page[number]={firstPageNumber}", first);

                // 10 pages by 10 page size
                result = await client.GetFullJsonResponseAsync($"{baseUrl}?page[size]=10");
                resultCount = ((JArray)result.Content["data"])?.Count;
                last = result.Content["links"]["last"].Value<string>();
                first = result.Content["links"]["first"].Value<string>();
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(10, resultCount);
                Assert.EndsWith($"{baseUrl}?page[size]=10&page[number]=10", last);
                Assert.EndsWith($"{baseUrl}?page[size]=10&page[number]={firstPageNumber}", first);

                // 5 pages by 20 default page size
                result = await client.GetFullJsonResponseAsync($"{baseUrl}");
                resultCount = ((JArray)result.Content["data"])?.Count;
                last = result.Content["links"]["last"].Value<string>();
                first = result.Content["links"]["first"].Value<string>();
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(20, resultCount);
                Assert.EndsWith($"{baseUrl}?page[number]=5", last);
                Assert.EndsWith($"{baseUrl}?page[number]={firstPageNumber}", first);

            }
        }

        [Fact(DisplayName = "Applies sorting when appropriate")]
        public async Task AppliesSorting()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/query/people?sort=age");
                _output.WriteLine(result.ToString());

                var ages = ((JArray)result["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();
                var sorted = ages.OrderBy(a => a).ToList();

                Assert.Equal(sorted, ages);
            }
        }


        [Theory(DisplayName = "Applies manual filtering when appropriate")]
        [InlineData("api/query/manual-typed/people", 10, null, 2)]
        [InlineData("api/query/manual-typed/people", 10, true, 2)]
        [InlineData("api/query/manual-typed/people", 10, false, 2)]
        [InlineData("api/query/manual-typed/people", 2, null, 10)]
        [InlineData("api/query/manual-typed/people", null, null, 10)]
        [InlineData("api/query/manual/paginate/people", 10, null, 2)]
        [InlineData("api/query/manual/paginate/people", 2, null, 10)]
        [InlineData("api/query/manual/paginate/people", null, null, 10)]
        [InlineData("api/query/manual/paginate/people", null, true, 10)]
        [InlineData("api/query/manual/paginate/people", null, false, 10)]
        [InlineData("api/query/manual/paginate/people?page[size]=4", null, true, 4)]
        [InlineData("api/query/manual/paginate/people?page[size]=4", null, null, 4)]
        // it should have 2 items and with page.size == 1 it should return just one of them
        [InlineData("api/query/manual/paginate/people?page[size]=1", 10, null, 1)]
        [InlineData("api/query/manual/paginate/people?page[size]=1", null, null, 1)]
        // we return only 10 persons in controller
        [InlineData("api/query/manual/paginate/people?page[size]=100", null, null, 10)] 
        public async Task AppliesManualFiltering(string query, int? minAge, bool? hideLastName, int expectedSize)
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                if (minAge.HasValue)
                {
                    query += $"{(query.Contains("?") ? "&" : "?")}filter[min-age]={minAge}";
                }

                if (hideLastName.HasValue)
                {
                    query += $"{(query.Contains("?") ? "&" : "?")}filter[hide-last-name]={hideLastName.Value}";
                }

                var result = await client.GetJsonResponseAsync(query);
                var data = (JArray) result["data"];

                var allLastNamesMissing = data.All(
                    p => string.IsNullOrWhiteSpace(p["attributes"]["last-name"].Value<string>()));

                // calculate count of returned records that satisfy condition
                var validCount = data.Count(p =>
                    !minAge.HasValue ||
                    p["attributes"]["age"].Value<int>() >= minAge.Value);

                var totalCount = ((JArray)result["data"]).Count;

                Assert.Equal(expectedSize, validCount);
                Assert.Equal(totalCount, validCount);

                if (hideLastName.HasValue)
                {
                    Assert.Equal(hideLastName.Value, allLastNamesMissing);
                }
            }
        }

        [Theory(DisplayName = "Applies manual filtering when appropriate")]
        [InlineData("api/query/manual/paginate/people", new string[0])]
        [InlineData("api/query/manual/paginate/people?include=car,job", new[] { "car", "corporation" })]
        [InlineData("api/query/manual/paginate/people?include=car", new[] { "car"})]
        [InlineData("api/query/manual/paginate/people?include=job", new[] { "corporation" })]
        public async Task AppliesManualIncluding(string query, string[] expectedIncludes)
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();

                var result = await client.GetJsonResponseAsync(query);

                // get all types that were included
                var includedTypes = ((JArray) result["included"])
                                    ?.Select(p => p["type"].Value<string>())
                                    .Distinct()
                                    .ToList() ?? new List<string>();

                Assert.Equal(includedTypes.Count, expectedIncludes.Length);

                foreach (var expectedInclude in expectedIncludes)
                {
                    Assert.Contains(expectedInclude, includedTypes);
                }
            }
        }


        [Theory(DisplayName = "Denies other attributes when HandlesQueryAttribute is specified")]
        [InlineData("api/broken/manual/disabledefault", "DisableDefaultIncludedAttribute shouldn't be used with HandlesQueryAttribute.")]
        [InlineData("api/broken/manual/allowsquery", "AllowsQueryAttribute shouldn't be used with HandlesQueryAttribute.")]
        public async Task DenyOtherAttributesForHandlesQuery(string query, string expectedTitle)
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();

                var response = await client.GetFullJsonResponseAsync(query);
                var error = response.Content["errors"][0];

                Assert.Equal("Saule.JsonApiException", error["code"].Value<string>());

                Assert.Equal(expectedTitle, error["title"].Value<string>());

                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }


        [Fact(DisplayName = "Applies filtering when appropriate")]
        public async Task AppliesFiltering()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/query/people?filter[last-name]=Russel");
                _output.WriteLine(result.ToString());

                var names = ((JArray)result["data"])
                    .Select(p => p["attributes"]["last-name"].Value<string>())
                    .ToList();

                var filtered = names.Where(a => a == "Russel").ToList();

                Assert.Equal(filtered.Count, names.Count);
            }
        }

        [Fact(DisplayName = "Does not apply sorting when not allowed")]
        public async Task AppliesSortingConditionally()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/people?sort=age");
                _output.WriteLine(result.ToString());

                var ages = ((JArray)result["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();
                var sorted = ages.OrderBy(a => a).ToList();

                Assert.NotEqual(sorted, ages);
            }
        }

        [Theory(DisplayName = "Always does sorting before pagination")]
        [InlineData("query/paginate")]
        [InlineData("paginate/query")]
        public async Task AppliesSortingBeforePagination(string path)
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result1 = await client.GetJsonResponseAsync($"api/{path}/people?sort=age");
                var result2 = await client.GetJsonResponseAsync($"api/{path}/people?sort=age&page[number]=1");
                _output.WriteLine(result1.ToString());
                _output.WriteLine(result2.ToString());

                var ages1 = ((JArray)result1["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();
                var ages2 = ((JArray)result2["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();

                var sorted = ages1.Concat(ages2).OrderBy(a => a).ToList();

                Assert.Equal(sorted, ages1.Concat(ages2).ToList());
            }
        }
        
        [Fact(DisplayName = "Gives useful error when you don't add ReturnsResourceAttribute")]
        public async Task GivesUsefulError()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var response = await client.GetFullJsonResponseAsync("api/broken/123/");
                _output.WriteLine(response.ToString());

                var result = response.Content;
                var error = result["errors"][0];

                Assert.Equal("https://github.com/joukevandermaas/saule/wiki",
                    error["links"]["about"].Value<string>());

                Assert.Equal("Saule.JsonApiException",
                    error["code"].Value<string>());

                Assert.Equal("Saule.JsonApiException: You must add a [ReturnsResourceAttribute] to action methods.",
                    error["detail"].Value<string>());

                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [Fact(DisplayName = "Passes through 4xx errors")]
        public async Task PassesThrough400Errors()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var response = await client.GetAsync("does/not/exist");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory(DisplayName = "Passes through two 4xx errors")]
        [InlineData("api/broken/errors")]
        // Passes through two 4xx errors when endpoint doesn't have any Resource specified
        [InlineData("api/broken/errorsNoResource")]
        public async Task PassesThroughTwoHttpErrors(string url)
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var response = await client.GetFullJsonResponseAsync(url);
                var errors = response.Content["errors"];

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Equal(2, errors.Count());
                Assert.Equal("Error 1.", errors[0]["title"]);
                Assert.Equal("Type 1", errors[0]["code"]);
                Assert.Equal("Error 2.", errors[1]["title"]);
                Assert.Equal("Type 2", errors[1]["code"]);
           }
        }

        [Fact(DisplayName = "Passes through one 400 error")]
        public async Task PassesThroughHttpError()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var response = await client.GetFullJsonResponseAsync("api/broken/error");
                var errors = response.Content["errors"];

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Equal(1, errors.Count());
                Assert.Equal("Error 1.", errors[0]["title"]);
                Assert.Equal("Type 1", errors[0]["code"]);
            }
        }

        [Fact(DisplayName = "Passes through one 400 error")]
        public async Task PassesThroughOneHttpException()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var response = await client.GetFullJsonResponseAsync("api/broken/exception");
                var errors = response.Content["errors"];

                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                Assert.Equal(1, errors.Count());
                Assert.Equal("An error has occurred.", errors[0]["title"]);
            }
        }

        [Fact(DisplayName = "Passes through HttpError")]
        public async Task PassesThroughHttpErrors()
        {
            using (var server = new NewSetupJsonApiServer())
            {
                var client = server.GetClient();
                var response = await client.GetFullJsonResponseAsync("api/broken");

                _output.WriteLine(response.Content.ToString());

                var errorText = response.Content["errors"][0]["title"].Value<string>();

                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
                Assert.Equal("Authorization has been denied for this request.", errorText);
            }
        }

        [Fact(DisplayName = "Works with delete/no content")]
        public async Task WorksForNoContent()
        {
            using (var server = new NewSetupJsonApiServer())
            {
                var client = server.GetClient();
                var result = await client.DeleteAsync("/api/companies/123");

                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            }
        }

        [Fact(DisplayName = "Uses user specified query filter expression for filtering")]
        public async Task UsesQueryFilterExpression()
        {
            var config = new JsonApiConfiguration();
            config.QueryFilterExpressions.SetExpression<string>((left, right) => left != right);

            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/query/people?filter[last-name]=Russel");
                _output.WriteLine(result.ToString());

                var names = ((JArray)result["data"])
                    .Select(p => p["attributes"]["last-name"].Value<string>())
                    .ToList();

                var filtered = names.Where(a => a != "Russel").ToList();

                Assert.Equal(filtered.Count, names.Count);
            }

        }
    }
}
