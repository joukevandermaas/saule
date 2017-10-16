using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Saule
{
    internal static class HttpRequestExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this HttpRequest uri)
        {
            return null;
        }
    }
}
