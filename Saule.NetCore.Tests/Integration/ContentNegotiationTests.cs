using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Http;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Integration
{
    public class ContentNegotiationTests
    {
        public class NewSetup : IClassFixture<NewSetupJsonApiServer>
        {
            private readonly NewSetupJsonApiServer _server;

            private readonly string _personContent = File.ReadAllText($"../../../Assets/person.json");

            public NewSetup(NewSetupJsonApiServer server, ITestOutputHelper output)
            {
                _server = server;
            }

            [Theory(DisplayName = "Servers MUST return content type 'application/vnd.api+json'")]
            [InlineData(Paths.SingleResource)]
            [InlineData(Paths.ResourceCollection)]
            public async Task MustReturnJsonApiContentType(string path)
            {
                var target = _server.GetClient();

                var result = await target.GetAsync(path);

                Assert.Equal("application/vnd.api+json", result.Content.Headers.ContentType.MediaType);
            }

            [Theory(DisplayName = "Servers MUST respond with '415 Not supported' to media type parameters in content-type header")]
            [InlineData("version", "1")]
            [InlineData("charset", "utf-8")]
            public async Task MustReturn415ToWrongContentTypeHeader(string key, string value)
            {
                var target = _server.GetClient();

                var mediaType = new MediaTypeHeaderValue(Constants.MediaType);
                mediaType.Parameters.Add(new NameValueHeaderValue(key, value));
                HttpContent content = new StringContent(_personContent);
                content.Headers.ContentType = mediaType;

                var result = await target.PostAsync(Paths.SingleResource, content);

                Assert.Equal(HttpStatusCode.UnsupportedMediaType, result.StatusCode);
            }

            [Theory(DisplayName = "Servers MUST respond with '406 Not acceptable' to media type parameters in accept header")]
            [InlineData("version", "1")]
            [InlineData("charset", "utf-8")]
            public async Task MustReturn406ToWrongAcceptHeader(string key, string value)
            {
                var target = _server.GetClient();

                var mediaType = new MediaTypeWithQualityHeaderValue(Constants.MediaType);
                mediaType.Parameters.Add(new NameValueHeaderValue(key, value));
                target.DefaultRequestHeaders.Accept.Clear();
                target.DefaultRequestHeaders.Accept.Add(mediaType);

                var result = await target.GetAsync(Paths.SingleResource);

                Assert.Equal(HttpStatusCode.NotAcceptable, result.StatusCode);
            }

            [Fact(DisplayName = "Should return OK if at least one Accept header does not have media type parameters")]
            public async Task MustReturn200OkForOneValidAccept()
            {
                var target = _server.GetClient();

                var mediaTypeWithParams = new MediaTypeWithQualityHeaderValue(Constants.MediaType);
                mediaTypeWithParams.Parameters.Add(new NameValueHeaderValue("charset", "utf-8"));

                var mediaTypeWithoutParams = new MediaTypeWithQualityHeaderValue(Constants.MediaType);

                target.DefaultRequestHeaders.Accept.Clear();
                target.DefaultRequestHeaders.Accept.Add(mediaTypeWithParams);
                target.DefaultRequestHeaders.Accept.Add(mediaTypeWithoutParams);

                var result = await target.GetAsync(Paths.SingleResource);

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            [Fact(DisplayName = "Should return OK for a static content request that does not have media type parameters")]
            public async Task MustReturn200OkForStaticContent()
            {
                var target = _server.GetClient();

                target.DefaultRequestHeaders.Accept.Clear();

                var result = await target.GetAsync(Paths.StaticText);

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            [Theory(DisplayName = "Should respond 400 Bad Request to invalid json api")]
            [InlineData("invalid-json-api.json")]
            [InlineData("invalid-json.json")]
            public async Task MustRespond400ForBadJsonApi(string filename)
            {
                using (var server = new NewSetupJsonApiServer(new JsonApiConfiguration()))
                {
                    var client = server.GetClient();
                    var mediaType = new MediaTypeHeaderValue(Constants.MediaType);
                    HttpContent content = GetFileAsString(filename);
                    content.Headers.ContentType = mediaType;

                    var result = await client.PostAsync("api/people/123", content);

                    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                }
            }

            private static StringContent GetFileAsString(string filename)
            {
                return new StringContent(File.ReadAllText($"../../../Assets/{filename}"));
            }
        }
    }
}
