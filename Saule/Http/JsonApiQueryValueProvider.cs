using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders.Providers;

namespace Saule.Http
{
    /// <summary>
    /// Provider that will convert json api filters from kebab case to pascal case, so WebApi model binder would be able to parse it
    /// </summary>
    public class JsonApiQueryValueProvider : NameValuePairsValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiQueryValueProvider"/> class.
        /// </summary>
        public JsonApiQueryValueProvider(HttpActionContext actionContext, CultureInfo culture)
            : base(() => ParseSauleQueryPairs(actionContext), culture)
        {
        }

        private static IEnumerable<KeyValuePair<string, string>> ParseSauleQueryPairs(HttpActionContext actionContext)
        {
            // convert key to pascal case
            return actionContext.ControllerContext.Request.GetQueryNameValuePairs()
                .Select(p => new KeyValuePair<string, string>(p.Key.ToPascalCase(), p.Value))
                .ToList();
        }
    }
}