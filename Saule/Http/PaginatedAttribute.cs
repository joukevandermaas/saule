using System;
using System.Collections;
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
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PaginatedAttribute : ActionFilterAttribute
    {
        private int _perPage = 10;

        /// <summary>
        /// The number of items to return per response.
        /// </summary>
        public int PerPage
        {
            get { return _perPage; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(PerPage), value, "Must have at least one item per page.");
                _perPage = value;
            }
        }

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
            var context = actionExecutedContext.Request.Properties[Constants.PaginationContextPropertyName]
                as PaginationContext;

            if (content != null)
            {
                content.Value = PaginationInterpreter.ApplyPaginationIfApplicable(context, content.Value);
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
