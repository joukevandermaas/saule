using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Queries;
using Saule.Queries.Pagination;
using Saule.Queries.Sorting;

namespace Saule.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AllowsQueryAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var context = new SortingContext(actionContext.Request.GetQueryNameValuePairs());

            var query = GetQueryContext(actionContext);

            query.Sorting = context;
            base.OnActionExecuting(actionContext);
        }

        private static QueryContext GetQueryContext(HttpActionContext actionContext)
        {
            var hasQuery = actionContext.Request.Properties.ContainsKey(Constants.QueryContextPropertyName);
            QueryContext query;

            if (hasQuery)
            {
                query = actionContext.Request.Properties[Constants.QueryContextPropertyName]
                    as QueryContext;
            }
            else
            {
                query = new QueryContext();
                actionContext.Request.Properties.Add(Constants.QueryContextPropertyName, query);
            }

            return query;
        }
    }
}
