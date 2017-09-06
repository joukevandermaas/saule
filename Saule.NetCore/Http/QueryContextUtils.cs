using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Saule.Queries;

namespace Saule.Http
{
    internal static class QueryContextUtils
    {
        internal static QueryContext GetQueryContext(ActionExecutingContext actionContext)
        {
            var hasQuery = actionContext.HttpContext.Items.ContainsKey(Constants.PropertyNames.QueryContext);
            QueryContext query;

            if (hasQuery)
            {
                query = actionContext.HttpContext.Items[Constants.PropertyNames.QueryContext]
                    as QueryContext;
            }
            else
            {
                query = new QueryContext();
                actionContext.HttpContext.Items.Add(Constants.PropertyNames.QueryContext, query);
            }

            return query;
        }
    }
}
