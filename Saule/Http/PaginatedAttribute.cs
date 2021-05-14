﻿using System;
using System.Linq;
using System.Net;
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
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class PaginatedAttribute : ActionFilterAttribute
    {
        private int? _perPage;
        private int? _queryPageSizeLimit;
        private int _firstPageNumber = 0;

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
        /// Gets or sets the first page number. Default value is 0
        /// </summary>
        public int FirstPageNumber
        {
            get
            {
                return _firstPageNumber;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(FirstPageNumber), value, "The first page should be more or equal to 0.");
                }

                _firstPageNumber = value;
            }
        }

        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var paginationContext = new PaginationContext(
                actionContext.Request.GetQueryNameValuePairs(),
                _perPage,
                _queryPageSizeLimit,
                _firstPageNumber);

            var query = GetQueryContext(actionContext);

            query.Pagination = paginationContext;
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
