using System.Linq;
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

        [Fact(DisplayName = "Default constructor uses DefaultUrlPathBuilder and no converters (obsolete)")]
        public async Task DefaultConstructorObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal(1, result["data"]["attributes"]["location"].Value<int>());

                result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/people/0/employer/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Url builder constructor generates those urls (obsolete)")]
        public async Task UrlBuilderConstructorObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter(
                new CanonicalUrlPathBuilder());

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/corporations/456/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Converter constructor uses that converter (obsolete)")]
        public async Task ConverterConstructorObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter(
                new StringEnumConverter());

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());
            }
        }

        [Fact(DisplayName = "Builder and converter constructor uses both of those (obsolete)")]
        public async Task BuilderAndConverterConstructorObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter(
                new CanonicalUrlPathBuilder(),
                new StringEnumConverter());

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());

                result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/corporations/456/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Default constructor uses DefaultUrlPathBuilder and no converters (new)")]
        public async Task DefaultConstructor()
        {
            using (var server = new NewSetupJsonApiServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal(1, result["data"]["attributes"]["location"].Value<int>());

                result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/people/0/employer/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Url builder constructor generates those urls (new)")]
        public async Task UrlBuilderConstructor()
        {
            var config = new JsonApiConfiguration
            {
                UrlPathBuilder = new CanonicalUrlPathBuilder()
            };

            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/corporations/456/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Converter constructor uses that converter (new)")]
        public async Task ConverterConstructor()
        {
            var config = new JsonApiConfiguration
            {
                JsonConverters = { new StringEnumConverter() }
            };

            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());
            }
        }

        [Fact(DisplayName = "Builder and converter constructor uses both of those (new)")]
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
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());

                result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://localhost/corporations/456/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Applies pagination when appropriate (obsolete)")]
        public async Task AppliesPaginationObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/");
                _output.WriteLine(result.ToString());

                Assert.Equal(12, (result["data"] as JArray)?.Count);
            }
        }

        [Fact(DisplayName = "Applies sorting when appropriate (obsolete)")]
        public async Task AppliesSortingObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/query/people?sort=age");
                _output.WriteLine(result.ToString());

                var ages = ((JArray)result["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();
                var sorted = ages.OrderBy(a => a).ToList();

                Assert.Equal(sorted, ages);
            }
        }

        [Fact(DisplayName = "Applies filtering when appropriate (obsolete)")]
        public async Task AppliesFilteringObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/query/people?filter[last-name]=Russel");
                _output.WriteLine(result.ToString());

                var names = ((JArray)result["data"])
                    .Select(p => p["attributes"]["last-name"].Value<string>())
                    .ToList();

                var filtered = names.Where(a => a == "Russel").ToList();

                Assert.Equal(filtered.Count, names.Count);
            }
        }

        [Fact(DisplayName = "Does not apply sorting when not allowed (obsolete)")]
        public async Task AppliesSortingConditionallyObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/people?sort=age");
                _output.WriteLine(result.ToString());

                var ages = ((JArray)result["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();
                var sorted = ages.OrderBy(a => a).ToList();

                Assert.NotEqual(sorted, ages);
            }
        }

        [Theory(DisplayName = "Always does sorting before pagination (obsolete)")]
        [InlineData("/query/paginate")]
        [InlineData("/paginate/query")]
        public async Task AppliesSortingBeforePaginationObsolete(string path)
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result1 = await client.GetJsonResponseAsync($"{path}/people?sort=age");
                var result2 = await client.GetJsonResponseAsync($"{path}/people?sort=age&page[number]=1");
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

        [Fact(DisplayName = "Gives useful error when you don't add ReturnsResourceAttribute (obsolete)")]
        public async Task GivesUsefulErrorObsolete()
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new ObsoleteSetupJsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/broken/123/");
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

        [Fact(DisplayName = "Applies pagination when appropriate (new)")]
        public async Task AppliesPagination()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/");
                _output.WriteLine(result.ToString());

                Assert.Equal(12, (result["data"] as JArray)?.Count);
            }
        }

        [Fact(DisplayName = "Applies sorting when appropriate (new)")]
        public async Task AppliesSorting()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/query/people?sort=age");
                _output.WriteLine(result.ToString());

                var ages = ((JArray)result["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();
                var sorted = ages.OrderBy(a => a).ToList();

                Assert.Equal(sorted, ages);
            }
        }

        [Fact(DisplayName = "Applies filtering when appropriate (new)")]
        public async Task AppliesFiltering()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/query/people?filter[last-name]=Russel");
                _output.WriteLine(result.ToString());

                var names = ((JArray)result["data"])
                    .Select(p => p["attributes"]["last-name"].Value<string>())
                    .ToList();

                var filtered = names.Where(a => a == "Russel").ToList();

                Assert.Equal(filtered.Count, names.Count);
            }
        }

        [Fact(DisplayName = "Does not apply sorting when not allowed (new)")]
        public async Task AppliesSortingConditionally()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/people?sort=age");
                _output.WriteLine(result.ToString());

                var ages = ((JArray)result["data"])
                    .Select(p => p["attributes"]["age"].Value<int>())
                    .ToList();
                var sorted = ages.OrderBy(a => a).ToList();

                Assert.NotEqual(sorted, ages);
            }
        }

        [Theory(DisplayName = "Always does sorting before pagination (new)")]
        [InlineData("/query/paginate")]
        [InlineData("/paginate/query")]
        public async Task AppliesSortingBeforePagination(string path)
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result1 = await client.GetJsonResponseAsync($"{path}/people?sort=age");
                var result2 = await client.GetJsonResponseAsync($"{path}/people?sort=age&page[number]=1");
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

        [Fact(DisplayName = "Gives useful error when you don't add ReturnsResourceAttribute (new)")]
        public async Task GivesUsefulError()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/broken/123/");
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

        [Fact(DisplayName = "Uses user specified query filter expression for filtering")]
        public async Task UsesQueryFilterExpression()
        {
            var config = new JsonApiConfiguration();
            config.QueryFilterExpressions.SetExpression<string>((left, right) => left != right);

            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/query/people?filter[last-name]=Russel");
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
