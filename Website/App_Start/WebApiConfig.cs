using Saule.Http;
using System.Web.Http;
using Saule.Serialization;

namespace Website
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var formatter = new JsonApiMediaTypeFormatter(
                new DefaultUrlPathBuilder("api/"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(formatter);
        }
    }

}