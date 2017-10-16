using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Saule.Queries;

namespace Saule
{
    internal static class UriExtensions
    {
        private static readonly Regex KeyRegex = new Regex(@"(.+)\[(.+)\]");

        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            // remove leading ? and split on &
            var pairString = uri.Query.StartsWith("?") ? uri.Query.Substring(startIndex: 1) : uri.Query;
            var pairs = pairString.Split('&');

            return pairs.Select((pair) =>
            {
                var keyValue = pair.Split('=');

                var key = ReformatKey(keyValue[0]);

                if (keyValue.Length == 1)
                {
                    // query string with no value, e.g. "?hidden"
                    return new KeyValuePair<string, string>(key, string.Empty);
                }

                return new KeyValuePair<string, string>(key, keyValue[1]);
            });
        }

        private static string ReformatKey(string key)
        {
            var result = KeyRegex.Match(key);
            return result.Success 
                ? $"{result.Groups[1].Value}.{result.Groups[2].Value}" 
                : key;
        }
    }
}
