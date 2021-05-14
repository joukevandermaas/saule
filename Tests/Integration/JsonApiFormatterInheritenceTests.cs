using System.Collections;
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
        
        [Fact(DisplayName = "If null ApiResourceProvider is returned, then we should expect an error")]
        public async Task RequiresApiResourceProviderInstance()
        {
            var config = new JsonApiConfiguration()
            {
                ApiResourceProviderFactory = new NullApiResourceProviderFactory()
            };
            using (var server = new NewSetupJsonApiServer(config))
            {
                var client = server.GetClient();
                // id 1 is mapped to circle object
                var result = await client.GetJsonResponseAsync("api/shape/1");
                _output.WriteLine(result.ToString());

                var errors = result["errors"];
                Assert.Equal(1, errors.Count());

                var error = errors[0];

                Assert.Equal("https://github.com/joukevandermaas/saule/wiki",
                    error["links"]["about"].Value<string>());

                Assert.Equal("Saule.JsonApiException",
                    error["code"].Value<string>());

                Assert.Equal("Saule.JsonApiException: ApiResourceProviderFactory returned null but it should always return an instance of IApiResourceProvider.",
                    error["detail"].Value<string>());
            }
        }
        
        [Fact(DisplayName = "Filter base shapes that have green color and sort them")]
        public async Task QueryGreenShapes()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/shapes?filter[color]=Green&sort=color");
                _output.WriteLine(result.ToString());

                var items = result["data"] as JArray;
                Assert.Equal(1, items.Count);

                var rectangle = items[0];
                
                ValidateRectangle(rectangle, "2");
            }
        }

        [Fact(DisplayName = "Filter base shapes that have purple color and sort them by id in descending order")]
        public async Task QueryPurpleShapes()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/shapes?filter[color]=Purple&sort=-id");
                _output.WriteLine(result.ToString());

                var items = result["data"] as JArray;
                Assert.Equal(2, items.Count);

                var circle1 = items[0];
                var circle2 = items[1];
                
                // since sort by id is desc, it should be backward
                ValidateCircle(circle1, "3");
                ValidateCircle(circle2, "1");
            }
        }
        
        [Fact(DisplayName = "Filter by property in the inherited object gives an error as property is missing in base object")]
        public async Task FilterInheritedObjectGivesError()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/shapes?filter[left]=10&sort=-id");
                _output.WriteLine(result.ToString());

                var errors = result["errors"];
                Assert.Equal(1, errors.Count());

                var error = errors[0];

                Assert.Equal("Saule.JsonApiException",
                    error["code"].Value<string>());

                Assert.Contains("Saule.JsonApiException: Attribute 'left' not found.",
                    error["detail"].Value<string>());
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
        
        [Fact(DisplayName = "Get a group and validation relationship")]
        public async Task GetGroup()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/groups");
                _output.WriteLine(result.ToString());

                var items = result["data"] as JArray;
                Assert.Equal(1, items.Count);

                var group = items[0];
                Assert.Equal("group", group["type"]);
                Assert.Equal("1", group["id"]);
                Assert.Equal("Group 1", group["attributes"]["name"]);

                var relationships = group["relationships"]["shapes"]["data"] as JArray;
                Assert.Equal(3, relationships.Count);
                Assert.Equal("circle", relationships[0]["type"]);
                Assert.Equal("1", relationships[0]["id"]);
                
                Assert.Equal("rectangle", relationships[1]["type"]);
                Assert.Equal("2", relationships[1]["id"]);
                
                Assert.Equal("circle", relationships[2]["type"]);
                Assert.Equal("3", relationships[2]["id"]);
            }
        }
        
        [Fact(DisplayName = "Get a group with included shapes and validate types and shapes itself")]
        public async Task GetGroupWithIncludes()
        {
            using (var server = CreateServer())
            {
                var client = server.GetClient();
                var result = await client.GetJsonResponseAsync("api/groups?include=shapes");
                _output.WriteLine(result.ToString());

                var items = result["data"] as JArray;
                var included = result["included"] as JArray; 
                Assert.Equal(1, items.Count);
                Assert.Equal(3, included.Count);
                
                var circle1 = included[0];
                var rectangle2 = included[1];
                var circle3 = included[2];
                
                ValidateCircle(circle1,"1");
                ValidateRectangle(rectangle2, "2");
                ValidateCircle(circle3,"3");
            }
        }

        
        private static void ValidateRectangle(JToken rectangle, string expectedId)
        {
            Assert.Equal("rectangle", rectangle["type"]);
            Assert.Equal(expectedId, rectangle["id"]);
            Assert.Equal("Green", rectangle["attributes"]["color"]);
            Assert.Equal(10, rectangle["attributes"]["left"].Value<int>());
            Assert.Equal(10, rectangle["attributes"]["top"].Value<int>());
            Assert.Equal(100, rectangle["attributes"]["width"].Value<int>());
            Assert.Equal(100, rectangle["attributes"]["height"].Value<int>());
            Assert.Equal(5, rectangle["attributes"].Count());
        }

        private static void ValidateCircle(JToken circle, string expectedId)
        {
            Assert.Equal("circle", circle["type"]);
            Assert.Equal(expectedId, circle["id"]);
            Assert.Equal("Purple", circle["attributes"]["color"]);
            Assert.Equal(42, circle["attributes"]["radius"].Value<decimal>());
            Assert.Equal(2, circle["attributes"].Count());
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
        
        public class NullApiResourceProviderFactory : IApiResourceProviderFactory
        {
            public IApiResourceProvider Create(HttpRequestMessage request)
            {
                return null;
            }
        }

        public class ShapeApiResourceProvider : IApiResourceProvider
        {
            public ApiResource Resolve(object dataObject)
            {
                var type = dataObject.GetType();

                if (type.IsEnumerable() || type.IsArray)
                    type = type.GetGenericTypeParameterOfCollection();
                
                if (type == typeof(Group))
                {
                    return new GroupResource();
                }

                if (type == typeof(Circle))
                {
                    return new CircleResource();
                }

                if (type == typeof(Rectangle))
                {
                    return new RectangleResource();
                }

                return new ShapeResource();
            }

            public ApiResource ResolveRelationship(object dataObject, ApiResource relationship)
            {
                if (relationship is ShapeResource)
                {
                    return Resolve(dataObject);
                }

                return new GroupResource();
            }
        }
    }
}
