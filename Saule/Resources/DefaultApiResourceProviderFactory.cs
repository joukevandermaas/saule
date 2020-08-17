using System.Net.Http;

namespace Saule.Resources
{
    /// <summary>
    /// Default provider factory that resolves attributes based on attributes and HttpRequestMessage
    /// </summary>
    public class DefaultApiResourceProviderFactory : IApiResourceProviderFactory
    {
        /// <inheritdoc/>
        public IApiResourceProvider Create(HttpRequestMessage request)
        {
            object resource;
            if (request.Properties.TryGetValue(Constants.PropertyNames.ResourceDescriptor, out resource) && resource is ApiResource apiResource)
            {
                return new DefaultApiResourceProvider(apiResource);
            }

            // if no resource was specified then we return empty provider that will always return null
            return new DefaultApiResourceProvider(null);
        }
    }
}
