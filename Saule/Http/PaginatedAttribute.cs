using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Filters;
using Saule.Queries;

namespace Saule.Http
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class PaginatedAttribute : ActionFilterAttribute
    {
        public PaginatedAttribute(int perPage)
        {
            if (perPage < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(perPage), perPage, "Must have at least one item per page.");
            }

            PerPage = perPage;
        }

        public int PerPage { get; }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var content = actionExecutedContext.Response.Content as ObjectContent;
            var queryable = content?.Value as IQueryable;

            if (queryable != null)
            {
                var filters = actionExecutedContext.Request.GetQueryNameValuePairs();
                var context = new PaginationInterpreter(queryable, filters, PerPage).Apply();
                content.Value = context.Result;
                actionExecutedContext.Request.Properties.Add(Constants.QueryContextName, context);
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
