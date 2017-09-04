using System;
using System.Collections.Generic;
using System.Linq;
using Saule.Queries.Filtering;
using Saule.Queries.Including;
using Saule.Queries.Pagination;
using Saule.Queries.Sorting;

namespace Saule.Queries
{
    internal class QueryContext
    {
        public PaginationContext Pagination { get; set; }

        public SortingContext Sorting { get; set; }

        public FilteringContext Filtering { get; set; }

        public IncludingContext Including { get; set; }

        public override string ToString()
        {
            var result = new List<string>
            {
                Pagination?.ToString(),
                Sorting?.ToString(),
                Filtering?.ToString(),
                Including?.ToString()
            };

            return string.Join("&", result.Where(c => !string.IsNullOrEmpty(c)));
        }
    }
}
