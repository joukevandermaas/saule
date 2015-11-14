using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Saule;
using Saule.Http;

namespace Tests.Helpers
{
    public class JsonApiServer : IDisposable
    {
        public HttpServer Server { get; }

        public JsonApiServer()
            : this(new JsonApiMediaTypeFormatter())
        {
        }

        public JsonApiServer(JsonApiMediaTypeFormatter formatter)
        {
            var config = new HttpConfiguration();
            config.Formatters.Add(formatter);
            config.Routes.MapHttpRoute(
                 name: "DefaultApi",
                 routeTemplate: "{controller}/{id}",
                 defaults: new { id = RouteParameter.Optional }
             );

            Server = new HttpServer(config);
        }

        public HttpClient GetClient()
        {
            var client = new HttpClient(Server);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType));
            client.BaseAddress = new Uri("http://example.com/");

            return client;
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
