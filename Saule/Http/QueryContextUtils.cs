using System.Web.Http.Controllers;
using Saule.Queries;

namespace Saule.Http
{
    internal static class QueryContextUtils
    {
        internal static QueryContext GetQueryContext(HttpActionContext actionContext)
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
