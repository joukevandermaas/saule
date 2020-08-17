using System.Net.Http;

namespace Saule.Resources
{
    /// <summary>
    /// Factory that creates IApiResourceProvider. It can be overriden at JsonApiConfiguration
    /// </summary>
    public interface IApiResourceProviderFactory
    {
        /// <summary>
        /// Creates ApiResourceProvider based on the current http request
        /// </summary>
        /// <param name="request">HttpRequest that is processsed at the moment</param>
        /// <returns>ApiResourceProvider instance bound to current request</returns>
        IApiResourceProvider Create(HttpRequestMessage request);
    }
}
