using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Saule.Http
{
    /// <summary>
    /// Attribute used to specify the api resource related to a controller action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ReturnsResourceAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceType">The type of the resource this controller action returns.</param>
        public ReturnsResourceAttribute(Type resourceType)
        {
            if (!resourceType.IsSubclassOf(typeof(ApiResource)))
                throw new ArgumentException("Resource types must inherit from Saule.ApiResource");
            Resource = resourceType.CreateInstance<ApiResource>();
        }

        /// <summary>
        /// The type of the resource this controller action returns.
        /// </summary>
        public ApiResource Resource { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.Properties.Add(Constants.RequestPropertyName, Resource);
            base.OnActionExecuting(actionContext);
        }
    }
}