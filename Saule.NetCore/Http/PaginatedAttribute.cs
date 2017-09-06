using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
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
        private int? _perPage;
        private int? _queryPageSizeLimit;

        /// <summary>
        /// Gets or sets the number of items to return per response.
        /// </summary>
        public int PerPage
        {
            get
            {
                return _perPage ?? Constants.QueryValues.ValueNotSpecified;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PerPage), value, "Must have at least one item per page.");
                }

                if (_queryPageSizeLimit.HasValue && value > _queryPageSizeLimit)
                {
                    throw new ArgumentOutOfRangeException(nameof(PageSizeLimit), value, "Items per page cannot be larger than the page size limit.");
                }

                _perPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum page size to accept from a URL query string.
        /// </summary>
        public int PageSizeLimit
        {
            get
            {
                return _queryPageSizeLimit ?? Constants.QueryValues.ValueNotSpecified;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PageSizeLimit), value, "Must have at least one item per page.");
                }

                if (_perPage.HasValue && value < _perPage)
                {
                    throw new ArgumentOutOfRangeException(nameof(PageSizeLimit), value, "PageSizeLimit cannot be smaller than page size.");
                }

                _queryPageSizeLimit = value;
            }
        }

        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var queryParams = actionContext.HttpContext.Request.GetQueryNameValuePairs().ToList();

            var paginationContext = new PaginationContext(
                queryParams,
                _perPage,
                _queryPageSizeLimit);

            var query = GetQueryContext(actionContext);

            query.Pagination = paginationContext;
            base.OnActionExecuting(actionContext);
        }

        private static QueryContext GetQueryContext(ActionExecutingContext actionContext)
        {
            var hasQuery = actionContext.HttpContext.Items.ContainsKey(Constants.PropertyNames.QueryContext);
            QueryContext query;

            if (hasQuery)
            {
                query = actionContext.HttpContext.Items[Constants.PropertyNames.QueryContext]
                    as QueryContext;
            }
            else
            {
                query = new QueryContext();
                actionContext.HttpContext.Items.Add(Constants.PropertyNames.QueryContext, query);
            }

            return query;
        }
    }
}
