using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
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
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var queryParams = actionContext.HttpContext.Request.GetQueryNameValuePairs().ToList();
            var queryContext = QueryContextUtils.GetQueryContext(actionContext);

            queryContext.Sorting = new SortingContext(queryParams);
            queryContext.Filtering = new FilteringContext(queryParams);

            if (queryContext.Including == null)
            {
                queryContext.Including = new IncludingContext(queryParams);
            }
            else
            {
                queryContext.Including.SetIncludes(queryParams);
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
