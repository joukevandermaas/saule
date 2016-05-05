using System;
using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Pagination
{
    internal class PaginationQuery
    {
        public PaginationQuery(PaginationContext context)
        {
            if (context == null)
            {
                return;
            }

            if (!context.ClientFilters.ContainsKey(Constants.QueryNames.PageNumber))
            {
                context.ClientFilters.Add(Constants.QueryNames.PageNumber, null);
            }

            int page;
            var isNumber = int.TryParse(context.ClientFilters[Constants.QueryNames.PageNumber] ?? string.Empty, out page);

            FirstPage = CreateQueryString(context.ClientFilters, 0);
            NextPage = CreateQueryString(context.ClientFilters, isNumber ? page + 1 : 1);
            PreviousPage = isNumber && page > 0
                ? CreateQueryString(context.ClientFilters, page - 1)
                : null;
        }

        public string FirstPage { get; }

        public string NextPage { get; }

        public string PreviousPage { get; }

        private static string CreateQueryString(
            IDictionary<string, string> clientFilters,
            int page)
        {
            // Go through, replacing the page.number query param's value with
            // the actual page. Preserves other query parameters, since they're
            // still needed to provide the correct pagination links.
            var queries = clientFilters.Select(kv =>
            {
                var key = kv.Key;
                var value = kv.Key == Constants.QueryNames.PageNumber ? page.ToString() : kv.Value;

                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                if (!key.Contains("."))
                {
                    return new { Key = key, Value = value };
                }

                var left = key.Substring(0, key.IndexOf(".", StringComparison.InvariantCulture));
                var right = key.Substring(left.Length + 1);
                return new { Key = left + $"[{right}]", Value = value };
            }).Where(k => k != null).ToList();

            return queries.Any()
                ? "?" + string.Join("&", queries.Select(kv => $"{kv.Key}={kv.Value}"))
                : null;
        }
    }
}
