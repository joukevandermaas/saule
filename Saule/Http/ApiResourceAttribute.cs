using System;

namespace Saule.Http
{
    /// <summary>
    /// Attribute used to specify the api resource related to a controller action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiResourceAttribute : Attribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceType">The type of the resource this controller action returns.</param>
        public ApiResourceAttribute(Type resourceType)
        {
            if (!resourceType.IsSubclassOf(typeof(ApiResource)))
                throw new ArgumentException("Resource types must inherit from Saule.ApiResource");
            ResourceType = resourceType;
        }

        /// <summary>
        /// The type of the resource this controller action returns.
        /// </summary>
        public Type ResourceType { get; }
    }
}