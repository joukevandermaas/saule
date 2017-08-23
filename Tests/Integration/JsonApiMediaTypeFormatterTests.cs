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
