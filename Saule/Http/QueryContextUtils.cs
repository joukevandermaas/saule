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

                // we validate if action has QueryContext parameter
                // and if it has it, then we pass it
                var parameters = actionContext.ActionDescriptor.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType == typeof(QueryContext))
                    {
                        actionContext.ActionArguments[parameter.ParameterName] = query;
                    }
                }
            }

            return query;
        }
    }
}
