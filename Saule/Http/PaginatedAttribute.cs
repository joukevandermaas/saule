using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Queries;
using Saule.Queries.Pagination;

namespace Saule.Http
{
    /// <summary>
    /// Indicates that the returned collection must be paginated. If the collection
    /// implements <see cref="IQueryable{T}"/>, the query will be executed efficiently.
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
            get
            {
                return _perPage;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PerPage), value, "Must have at least one item per page.");
                }

                _perPage = value;
            }
        }

        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var context = new PaginationContext(
                actionContext.Request.GetQueryNameValuePairs(),
                PerPage);

            var query = GetQueryContext(actionContext);

            query.Pagination = context;
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
