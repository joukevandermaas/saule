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

        [Fact(DisplayName = "Default constructor uses DefaultUrlPathBuilder and no converters")]
        public async Task DefaultConstructor()
        {
            var formatter = new JsonApiMediaTypeFormatter();

            using (var server = new JsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal(1, result["data"]["attributes"]["location"].Value<int>());

                result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://example.com/people/0/employer/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Url builder constructor generates those urls")]
        public async Task UrlBuilderConstructor()
        {
            var formatter = new JsonApiMediaTypeFormatter(
                new CanonicalUrlPathBuilder());

            using (var server = new JsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://example.com/corporations/456/", relatedUrl);
            }
        }

        [Fact(DisplayName = "Converter constructor uses that converter")]
        public async Task ConverterConstructor()
        {
            var formatter = new JsonApiMediaTypeFormatter(
                new StringEnumConverter());

            using (var server = new JsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());
            }
        }

        [Fact(DisplayName = "Builder and converter constructor uses both of those")]
        public async Task BuilderAndConverterConstructor()
        {
            var formatter = new JsonApiMediaTypeFormatter(
                new CanonicalUrlPathBuilder(),
                new StringEnumConverter());

            using (var server = new JsonApiServer(formatter))
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("/companies/456/");
                _output.WriteLine(result.ToString());

                Assert.Equal("National", result["data"]["attributes"]["location"].Value<string>());

                result = await client.GetJsonResponseAsync("/people/");
                _output.WriteLine(result.ToString());

                var relatedUrl = result["data"][0]["relationships"]["job"]["links"]["related"]
                    .Value<string>();
                Assert.Equal("http://example.com/corporations/456/", relatedUrl);
            }
        }
    }
}
