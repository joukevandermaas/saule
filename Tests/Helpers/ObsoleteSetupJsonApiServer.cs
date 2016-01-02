using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Owin.Testing;
using Owin;
using Saule;
using Saule.Http;

namespace Tests.Helpers
{
    public class ObsoleteSetupJsonApiServer : IDisposable
    {
        private readonly TestServer _server;

        public ObsoleteSetupJsonApiServer()
            : this(new JsonApiMediaTypeFormatter())
        {
        }

        internal ObsoleteSetupJsonApiServer(JsonApiMediaTypeFormatter formatter)
        {
            var config = new HttpConfiguration();
            config.Formatters.Clear();
            config.Formatters.Add(formatter);
            config.MapHttpAttributeRoutes(new DefaultDirectRouteProvider());

            _server = TestServer.Create(builder =>
            {
                builder.UseWebApi(config);
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
