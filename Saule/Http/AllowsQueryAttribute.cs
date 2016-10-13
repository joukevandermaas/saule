using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Queries;
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
            var sorting = new SortingContext(queryParams);
            var filtering = new FilteringContext(queryParams);
            var including = new IncludingContext(queryParams);

            var queryContext = GetQueryContext(actionContext);

            queryContext.Sorting = sorting;
            queryContext.Filtering = filtering;
            queryContext.Including = including;

            base.OnActionExecuting(actionContext);
        }

        private static QueryContext GetQueryContext(HttpActionContext actionContext)
        {
            var hasQuery = actionContext.Request.Properties.ContainsKey(Constants.PropertyNames.QueryContext);
            QueryContext query;

            if (hasQuery)
            {
                query = actionContext.Request.Properties[Constants.PropertyNames.QueryContext]
                    as QueryContext;
            }
            else
            {
                query = new QueryContext();
                actionContext.Request.Properties.Add(Constants.PropertyNames.QueryContext, query);
            }

            return query;
        }
    }
}
