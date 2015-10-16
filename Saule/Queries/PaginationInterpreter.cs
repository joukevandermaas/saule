using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Saule.Queries
{
    internal class PaginationInterpreter
    {
        private IDictionary<string, string> _filters;
        private readonly int _perPage;
        private readonly IQueryable _queryable;

        public PaginationInterpreter(IQueryable queryable, IEnumerable<KeyValuePair<string, string>> filters, int perPage)
        {
            _queryable = queryable;
            _filters = filters.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value.ToLowerInvariant());
            _perPage = perPage;
        }

        public PaginationContext Apply()
        {
            var page = GetNumber();
            var filtered = _queryable.ApplyQuery(QueryMethods.Skip, page * _perPage) as IQueryable;

            filtered = filtered.ApplyQuery(QueryMethods.Take, _perPage) as IQueryable;

            return new PaginationContext(filtered, page, _perPage);
        }

        private int GetNumber()
        {
            if (!_filters.ContainsKey("page.number")) return 0;

            int result;
            var isNumber = int.TryParse(_filters["page.number"], out result);

            return isNumber ? result : 0;
        }
    }

    internal class PaginationContext
    {
        public PaginationContext(IQueryable result, int page, int perPage)
        {
            Result = result;
            Page = page;
            PerPage = perPage;
        }

        public IQueryable Result { get; }
        public int Page { get; }
        public int PerPage { get; }
    }
}
