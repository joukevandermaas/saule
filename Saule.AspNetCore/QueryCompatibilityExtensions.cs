using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Saule
{
    internal static class QueryCompatibilityExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this HttpRequest request)
        {
            return request.Query.SelectMany(
                x => x.Value,
                (col, value) => new KeyValuePair<string, string>(
                    ProcessKey(col.Key),
                    value));
        }

        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this Uri uri)
        {
            uri.ThrowIfNull(nameof(uri));

            var query = QueryHelpers.ParseQuery(uri.Query);

            return query.SelectMany(
                x => x.Value,
                (col, value) => new KeyValuePair<string, string>(
                    ProcessKey(col.Key),
                    value));
        }

        private static string ProcessKey(string key)
        {
            key.ThrowIfNull(nameof(key));

            /* in classic webapi, keys like "filter[location]" are transformed into "filter.location"
             * somehow... by GetQueryNameValuePairs()
             */

            if (key.Contains("["))
            {
                key = key.Replace("[", ".")
                    .Replace("]", string.Empty);
            }

            return key;
        }
    }
}
