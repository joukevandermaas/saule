using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Configuration;
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
        private int _perPage = PaginationConfig.DefaultPageSize;
        private int? _queryPageSizeLimit = PaginationConfig.DefaultPageSizeLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedAttribute"/> class.        
        /// </summary>
        public PaginatedAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedAttribute"/> class.
        /// </summary>
        /// <param name="queryPageSizeLimit">Maximum page size to accept from a URL query string.</param>
        public PaginatedAttribute(int queryPageSizeLimit)
        {
            _queryPageSizeLimit = queryPageSizeLimit;
        }

        /// <summary>
        /// Gets or sets the number of items to return per response.
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

                if (value > QueryPageSizeLimit)
                {
                    throw new ArgumentOutOfRangeException(nameof(PerPage), value, "Page size limit cannot be smaller than page size.");
                }

                _perPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum page size the client can specify the page size with a "page[size]" query parameter.
        /// </summary>
        /// <remarks>
        /// Null values indicate the page[size] parameter will be ignored.
        /// </remarks>
        private int? QueryPageSizeLimit
        {
            get
            {
                return _queryPageSizeLimit;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PerPage), value, "Must have at least one item per page.");
                }

                if (value < PerPage)
                {
                    throw new ArgumentOutOfRangeException(nameof(QueryPageSizeLimit), value, "Page size limit cannot be smaller than page size.");
                }

                _queryPageSizeLimit = value;
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
                PerPage,
                QueryPageSizeLimit.HasValue);

            if (QueryPageSizeLimit.HasValue && context.PerPage > QueryPageSizeLimit)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var query = GetQueryContext(actionContext);

            query.Pagination = context;
            base.OnActionExecuting(actionContext);
        }

        private static QueryContext GetQueryContext(HttpActionContext actionContext)
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
