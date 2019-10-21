using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saule.Queries.Pagination
{
    internal class PagedResultQuery
    {
        private PaginationContext _context;

        public PagedResultQuery(PaginationContext context)
        {
            _context = context;
        }

        public object Apply(object value)
        {
            if (!IsPagedResult(value))
            {
                return value;
            }

            // we are getting total count from PagedResult
            // and then unwrape actual IEnumerable and return it
            _context.TotalResultsCount = GetTotalResultsCount(value);
            var actualItems = UnwrapPagedResult(value);

            return actualItems;
        }

        private static bool IsPagedResult(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!obj.GetType().IsGenericType
                || obj.GetType().GetGenericTypeDefinition() != typeof(PagedResult<>))
            {
                return false;
            }

            return true;
        }

        private static object UnwrapPagedResult(object obj)
        {
            return obj.GetValueOfProperty(nameof(PagedResult<object>.Items), true);
        }

        private static int GetTotalResultsCount(object obj)
        {
            return (int)obj.GetValueOfProperty(nameof(PagedResult<object>.TotalResultsCount));
        }
    }
}
