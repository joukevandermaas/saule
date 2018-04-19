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
    }
}
