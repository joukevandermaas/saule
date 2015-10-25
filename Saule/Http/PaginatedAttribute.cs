using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Queries;

namespace Saule.Http
{
    /// <summary>
    /// Indicates that the returned queryable must be paginated. Only works if the action
    /// method returns <see cref="IQueryable{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class PaginatedAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="perPage">The number of items to return per response.</param>
        public PaginatedAttribute(int perPage)
        {
            if (perPage < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(perPage), perPage, "Must have at least one item per page.");
            }

            PerPage = perPage;
        }

        /// <summary>
        /// The number of items to return per response.
        /// </summary>
        public int PerPage { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var context = new PaginationContext(
                actionContext.Request.GetQueryNameValuePairs(),
                PerPage);
            actionContext.Request.Properties.Add(Constants.PaginationContextPropertyName, context);
            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
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
