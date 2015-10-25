using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
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

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var context = new PaginationContext(
                actionContext.Request.GetQueryNameValuePairs(),
                PerPage);
            actionContext.Request.Properties.Add(Constants.PaginationContextPropertyName, context);
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var content = actionExecutedContext.Response.Content as ObjectContent;
            var queryable = content?.Value as IQueryable;
            var context = actionExecutedContext.Request.Properties[Constants.PaginationContextPropertyName]
                as PaginationContext;

            if (queryable != null)
            {
                content.Value = new PaginationInterpreter(context).Apply(queryable);
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
