using System.Net.Http;

namespace Saule.Resources
{
    /// <summary>
    /// Factory that creates IApiResourceProvider. It can be overriden at JsonApiConfiguration
    /// </summary>
    public interface IApiResourceProviderFactory
    {
        /// <summary>
        /// Creates ApiResourceProvider based on the current http request. It should always return a provider. Provider itself might return null ApiResource but if provider itself is null, then the error would be thrown
        /// </summary>
        /// <param name="request">HttpRequest that is processsed at the moment</param>
        /// <returns>ApiResourceProvider instance bound to current request</returns>
        IApiResourceProvider Create(HttpRequestMessage request);
    }
}
