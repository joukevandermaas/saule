using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Saule.Http
{
    /// <summary>
    /// Attribute used to specify the api resource related to a controller action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ReturnsResourceAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnsResourceAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">The type of the resource this controller action returns.</param>
        public ReturnsResourceAttribute(Type resourceType)
        {
            if (!resourceType.IsSubclassOf(typeof(ApiResource)))
            {
                throw new ArgumentException("Resource types must inherit from Saule.ApiResource");
            }

            Resource = resourceType.CreateInstance<ApiResource>();
        }

        /// <summary>
        /// Gets the type of the resource this controller action returns.
        /// </summary>
        public ApiResource Resource { get; }

        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var accept = actionContext.Request.Headers.Accept
                .Where(a => a.MediaType == Constants.MediaType);
            if (accept.Count() > 0 && accept.All(a => a.Parameters.Any()))
            {
                // no json api media type without parameters
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
            }

            var contentType = actionContext.Request.Content?.Headers?.ContentType;
            if (contentType != null && contentType.Parameters.Any())
            {
                // client is sending json api media type with parameters
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }

            actionContext.Request.Properties.Add(Constants.PropertyNames.ResourceDescriptor, Resource);
            base.OnActionExecuting(actionContext);
        }
    }
}