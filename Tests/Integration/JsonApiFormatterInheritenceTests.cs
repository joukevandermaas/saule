using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Http;
using Saule.Resources;
using Saule.Serialization;
using Tests.Helpers;
using Tests.Models.Inheritance;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Integration
{
    public class JsonApiFormatterInheritenceTests
    {
        private readonly ITestOutputHelper _output;

        public JsonApiFormatterInheritenceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Default endpoint that returns all items")]
        public async Task GetAllItems()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/shapes");
                _output.WriteLine(result.ToString());

                var items = result["data"] as JArray;
                Assert.Equal(3, items.Count);

                var circle1 = items[0];
                var rectangle2 = items[1];
                var circle3 = items[2];
                
                ValidateCircle(circle1,"1");
                ValidateRectangle(rectangle2, "2");
                ValidateCircle(circle3,"3");
            }
        }

        [Fact(DisplayName = "Get a specific circle and validate the attributes")]
        public async Task GetSpecificCircle()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                // id 1 is mapped to circle object
                var result = await client.GetJsonResponseAsync("api/shape/1");
                _output.WriteLine(result.ToString());

                var circle = result["data"];

                ValidateCircle(circle, "1");
            }
        }
        
        [Fact(DisplayName = "Get a specific rectangle and validate the attributes")]
        public async Task GetSpecificRectangle()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                // id 2 is mapped to rectangle object
                var result = await client.GetJsonResponseAsync("api/shape/2");
                _output.WriteLine(result.ToString());

                var rectangle = result["data"];

                ValidateRectangle(rectangle, "2");
            }
        }

        
        private static void ValidateRectangle(JToken rectangle2, string expectedId)
        {
            Assert.Equal("rectangle", rectangle2["type"]);
            Assert.Equal(expectedId, rectangle2["id"]);
            Assert.Equal("Purple", rectangle2["attributes"]["color"]);
            Assert.Equal(10, rectangle2["attributes"]["left"].Value<int>());
            Assert.Equal(10, rectangle2["attributes"]["top"].Value<int>());
            Assert.Equal(100, rectangle2["attributes"]["width"].Value<int>());
            Assert.Equal(100, rectangle2["attributes"]["height"].Value<int>());
            Assert.Equal(5, rectangle2["attributes"].Count());
        }

        private static void ValidateCircle(JToken circle1, string expectedId)
        {
            Assert.Equal("circle", circle1["type"]);
            Assert.Equal(expectedId, circle1["id"]);
            Assert.Equal("Purple", circle1["attributes"]["color"]);
            Assert.Equal(42, circle1["attributes"]["radius"].Value<decimal>());
            Assert.Equal(2, circle1["attributes"].Count());
        }

        private NewSetupJsonApiServer CreateServer()
        {
            var config = new JsonApiConfiguration()
            {
                ApiResourceProviderFactory = new ShapeApiResourceProviderFactory()
            };
            return new NewSetupJsonApiServer(config);
        }

        public class ShapeApiResourceProviderFactory : IApiResourceProviderFactory
        {
            public IApiResourceProvider Create(HttpRequestMessage request)
            {
                return new ShapeApiResourceProvider();
            }
        }

        public class ShapeApiResourceProvider : IApiResourceProvider
        {
            public ApiResource Resolve(object dataObject)
            {
                switch (dataObject)
                {
                    case Circle _: return new CircleResource();
                    case Rectangle _: return new RectangleResource();
                    default: return new ShapeResource();
                }
            }
        }
    }
}
