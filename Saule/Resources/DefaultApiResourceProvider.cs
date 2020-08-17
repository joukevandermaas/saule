namespace Saule.Resources
{
    /// <summary>
    /// Default ApiResourceProvider that always return the same ApiResource that is bound to current request regardless of the object type
    /// </summary>
    public class DefaultApiResourceProvider : IApiResourceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApiResourceProvider"/> class.
        /// </summary>
        /// <param name="apiResource">default ApiResource based on the controller's attribute</param>
        public DefaultApiResourceProvider(ApiResource apiResource)
        {
            ApiResource = apiResource;
        }

        /// <summary>
        /// Gets current api resource
        /// </summary>
        protected ApiResource ApiResource { get; }

        /// <inheritdoc/>
        public virtual ApiResource Resolve(object dataObject)
        {
            return ApiResource;
        }
    }
}
