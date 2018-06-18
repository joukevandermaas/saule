using System.Threading.Tasks;
using Saule.Http;
using Saule.Serialization;
using Tests.Helpers;
using Xunit;

namespace Tests.Integration
{
    public class PropertyNameConverterTests
    {
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
