using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Tests.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<JObject> GetJsonResponseAsync(this HttpClient client, string url)
        {
            var result = await client.GetAsync(url);
            var content = JObject.Parse(await result.Content.ReadAsStringAsync());

            return content;
        }

        public static async Task<JsonResponse> GetFullJsonResponseAsync(this HttpClient client, string url)
        {
            
            var result = await client.GetAsync(url);
            var content = JObject.Parse(await result.Content.ReadAsStringAsync());

            return new JsonResponse(content, result.StatusCode);
        }

        public class JsonResponse
        {
            public JsonResponse(JObject content, HttpStatusCode statusCode)
            {
                Content = content;
                StatusCode = statusCode;
            }

            public JObject Content { get; }

            public HttpStatusCode StatusCode { get; }

            public override string ToString()
            {
                return Content.ToString();
            }
        }
    }
}
