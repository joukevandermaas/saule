using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Saule
{
    internal static class HttpRequestExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this HttpRequest request)
        {
            return request.Query.Select(s => new KeyValuePair<string, string>(s.Key, s.Value));
        }
    }
}
