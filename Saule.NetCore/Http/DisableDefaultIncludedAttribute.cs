using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Saule.Queries.Including;

namespace Saule.Http
{
    /// <summary>
    /// Indicates that the action should not include related data by default.
    ///
    /// This attribute is only relevant when requesting a resource without an
    /// explicit include parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DisableDefaultIncludedAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var queryContext = QueryContextUtils.GetQueryContext(actionContext);

            if (queryContext.Including == null)
            {
                queryContext.Including = new IncludingContext();
            }

            queryContext.Including.DisableDefaultIncluded = true;

            base.OnActionExecuting(actionContext);
        }
    }
}
