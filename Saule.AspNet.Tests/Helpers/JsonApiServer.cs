using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Owin.Testing;
using Owin;
using Saule.Http;

namespace Saule.AspNet.Tests.Helpers
{
    public class JsonApiServer : IDisposable
    {
        private readonly TestServer _server;

        public JsonApiServer()
            : this(null)
        {
        }

        internal JsonApiServer(JsonApiConfiguration config)
        {
            var httpConfig = new HttpConfiguration();
            if (config == null)
            {
                httpConfig.ConfigureJsonApi();
            }
            else
            {
                httpConfig.ConfigureJsonApi(config);
            }

            httpConfig.MapHttpAttributeRoutes(new DefaultDirectRouteProvider());

            _server = TestServer.Create(builder =>
            {
                builder.UseWebApi(httpConfig);
            });
        }

        public HttpClient GetClient()
        {
            var client = _server.HttpClient;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType));

            return client;
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}