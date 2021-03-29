using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Queries;
using Saule.Queries.Fieldset;
using Saule.Queries.Filtering;
using Saule.Queries.Including;
using Saule.Queries.Sorting;

namespace Saule.Http
{
    /// <summary>
    /// Indicates that filter, paging, include and sorting should be parsed. But they won't be automatically applied to WebApi action
    /// and action should handle them manually
    /// </summary>
    public class HandlesQueryAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            EnsureAttributeNotSpecified<DisableDefaultIncludedAttribute>(actionContext.ActionDescriptor, actionContext);
            EnsureAttributeNotSpecified<AllowsQueryAttribute>(actionContext.ActionDescriptor, actionContext);

            var queryParams = actionContext.Request.GetQueryNameValuePairs().ToList();
            var queryContext = QueryContextUtils.GetQueryContext(actionContext);

            queryContext.IsHandledQuery = true;
            queryContext.Sort = new SortContext(queryParams);
            queryContext.Filter = new FilterContext(queryParams);
            queryContext.Fieldset = new FieldsetContext(queryParams);

            if (queryContext.Include == null)
            {
                queryContext.Include = new IncludeContext(queryParams);
            }
            else
            {
                queryContext.Include.SetIncludes(queryParams);
            }

            base.OnActionExecuting(actionContext);
        }

        private void EnsureAttributeNotSpecified<T>(HttpActionDescriptor descriptor, HttpActionContext actionContext)
            where T : class
        {
            if (descriptor.GetCustomAttributes<T>().Any())
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    new HttpError(
                        new JsonApiException(ErrorType.Server, $"{typeof(T).Name} shouldn't be used with {typeof(HandlesQueryAttribute).Name}"),
                        true));
            }
        }
    }
}
