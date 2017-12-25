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
    /// Indicates that filter, paging, include and sorting should be parsed. But they won't be automatically applied to WebApi action
    /// and action should handle them manually
    /// </summary>
    public class AllowsManuallyHandledQueryAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var queryParams = actionContext.Request.GetQueryNameValuePairs().ToList();
            var queryContext = QueryContextUtils.GetQueryContext(actionContext);

            queryContext.IsManuallyHandledQuery = true;
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

            // we validate if action has QueryContext parameter
            // and if it has it, then we pass it
            var parameters = actionContext.ActionDescriptor.GetParameters();
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == typeof(QueryContext))
                {
                    actionContext.ActionArguments[parameter.ParameterName] = queryContext;
                }
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
