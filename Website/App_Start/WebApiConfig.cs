using Saule.Http;
using System.Web.Http;
using Saule.Serialization;

namespace Website
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.ConfigureJsonApi(new JsonApiConfiguration
            {
                UrlPathBuilder = new DefaultUrlPathBuilder("/api")
            });
        }
    }

}