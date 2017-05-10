using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Queries.Filtering;
using Saule.Queries.Including;
using Saule.Queries.Sorting;

namespace Saule.Http
{
    /// <summary>
    /// Describes how the serializer should treat included resources when no included query is sent.
    /// </summary>
    public enum IncludeByDefault
    {
        /// <summary>
        /// All related resources that are present should be included.
        /// </summary>
        All,

        /// <summary>
        /// No related resources should be included.
        /// </summary>
        None
    }

    /// <summary>
    /// Indicates that the target action supports the use of the include query param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AllowsIncludeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets how the serializer should treat included resources when no included query is sent.
        /// </summary>
        public IncludeByDefault Default { get; set; } = IncludeByDefault.All;

        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var queryParams = actionContext.Request.GetQueryNameValuePairs().ToList();
            var queryContext = QueryContextUtils.GetQueryContext(actionContext);

            if (queryContext.Including == null)
            {
                queryContext.Including = new IncludingContext(queryParams);
            }
            else
            {
                queryContext.Including.SetIncludes(queryParams);
            }

            queryContext.Including.DisableDefaultIncluded = Default == IncludeByDefault.None;

            base.OnActionExecuting(actionContext);
        }
    }
}
