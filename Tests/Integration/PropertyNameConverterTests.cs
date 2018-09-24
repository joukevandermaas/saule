using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Saule.Http;
using Saule.Serialization;
using Tests.Helpers;
using Xunit;

namespace Tests.Integration
{
    public class PropertyNameConverterTests
    {
        [Fact(DisplayName = "Deserializes complex models correctly")]
        public async Task DeserializeWorks()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration
            {
                PropertyNameConverter = new CamelCasePropertyNameConverter()
            }))
            {
                var client = server.GetClient();

                var getResult = await client.GetJsonResponseAsync("api/people/123");
                getResult.Remove("included");

                var content = new StringContent(getResult.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");
                var postResultMessage = await client.PostAsync("api/people/123", content);

                var postResult = JObject.Parse(await postResultMessage.Content.ReadAsStringAsync());
                postResult.Remove("included");

                Assert.True(JToken.DeepEquals(getResult, postResult));
            }
        }

        [Fact(DisplayName = "CamelCase constuctor generates attributes in camelCase")]
        public async Task CamelCaseConstructor()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration
            {
                PropertyNameConverter = new CamelCasePropertyNameConverter()
            }))
            {
                var client = server.GetClient();

                var result = await client.GetJsonResponseAsync("api/people/123");

                Assert.Null(result["data"]["attributes"]["FirstName"]);
                Assert.NotNull(result["data"]["attributes"]["firstName"]);
            }
        }

        [Fact(DisplayName = "Relationship names are converted correctly")]
        public async Task CamelCaseRelationships()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration
            {
                PropertyNameConverter = new CamelCasePropertyNameConverter()
            }))
            {
                var client = server.GetClient();

                var result = await client.GetJsonResponseAsync("api/people/123");

                var relationships = result["data"]["relationships"];

                Assert.NotNull(relationships["secretFriends"]);
                Assert.NotNull(relationships["familyMembers"]);

                Assert.Null(relationships["secret-friends"]);
                Assert.Null(relationships["family-members"]);
            }
        }

        [Fact(DisplayName = "Nested properties in attributes are converted correctly")]
        public async Task CamelCaseNestedAttrs()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration
            {
                PropertyNameConverter = new CamelCasePropertyNameConverter()
            }))
            {
                var client = server.GetClient();

                var result = await client.GetJsonResponseAsync("api/people/123");

                var address = result["data"]["attributes"]["address"];

                Assert.NotNull(address["streetName"]);
                Assert.NotNull(address["zipCode"]);
            }
        }

        [Fact(DisplayName = "Meta hash properties are converted correctly")]
        public async Task CamelCaseMeta()
        {
            using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration
            {
                PropertyNameConverter = new CamelCasePropertyNameConverter()
            }))
            {
                var client = server.GetClient();

                var result = await client.GetJsonResponseAsync("api/people/123");

                var meta = result["meta"];

                Assert.NotNull(meta["numberOfFriends"]);
                Assert.NotNull(meta["numberOfFamilyMembers"]);
            }
        }
    }
}
