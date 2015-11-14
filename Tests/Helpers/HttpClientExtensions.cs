using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Tests.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<JObject> GetJsonResponseAsync(this HttpClient client, string url)
        {
            return JObject.Parse(await (await client.GetAsync(url)).Content.ReadAsStringAsync());
        }
    }
}
