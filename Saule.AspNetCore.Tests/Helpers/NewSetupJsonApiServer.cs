using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Saule;
using Saule.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tests.Helpers
{
    public class NewSetupJsonApiServer : IDisposable
    {
        private readonly TestServer _server;

        public NewSetupJsonApiServer()
            : this(new JsonApiConfiguration())
        {
        }

        internal NewSetupJsonApiServer(JsonApiConfiguration config)
        {
            _server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s
                    .AddSingleton(config))
                .UseStartup<Startup>());
        }

        public HttpClient GetClient()
        {
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType));

            return client;
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddJsonApi();
            }

            public void Configure(IApplicationBuilder app,
                IHostingEnvironment env,
                ILoggerFactory loggerFactory)
            {
                app.UseMvcWithDefaultRoute();
            }
        }
    }
}