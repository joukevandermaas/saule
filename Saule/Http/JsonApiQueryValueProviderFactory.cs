using System.Globalization;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders;

namespace Saule.Http
{
    /// <summary>
    /// Value provider factory for JsonApiQueryValueProvider
    /// </summary>
    public class JsonApiQueryValueProviderFactory : ValueProviderFactory, IUriValueProviderFactory
    {
        /// <inheritdoc/>
        public override IValueProvider GetValueProvider(HttpActionContext actionContext)
        {
            return new JsonApiQueryValueProvider(actionContext, CultureInfo.InvariantCulture);
        }
    }
}