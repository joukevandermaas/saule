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
    /// Indicates that the returned collection can be filtered by through query parameters.
    /// If the collection implements <see cref="IQueryable{T}"/>, the query will be executed
    /// efficiently.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AllowsQueryAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var queryParams = actionContext.Request.GetQueryNameValuePairs().ToList();
            var queryContext = QueryContextUtils.GetQueryContext(actionContext);

            queryContext.Sort = new SortContext(queryParams);
            queryContext.Filter = new FilterContext(queryParams);

            if (queryContext.Include == null)
            {
                queryContext.Include = new IncludeContext(queryParams);
            }
            else
            {
                queryContext.Include.SetIncludes(queryParams);
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
