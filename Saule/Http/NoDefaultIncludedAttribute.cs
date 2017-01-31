using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Saule.Http
{
    /// <summary>
    /// Indicates that the action should not include related data by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NoDefaultIncludedAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var queryContext = QueryContextUtils.GetQueryContext(actionContext);
            queryContext.IncludedDefault = false;

            base.OnActionExecuting(actionContext);
        }
    }
}
