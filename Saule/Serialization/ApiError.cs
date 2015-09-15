using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Saule.Serialization
{
    internal class ApiError
    {
        public ApiError(Exception ex)
        {
            Title = ex.Message;
            Detail = ex.ToString();
            Code = ex.GetType().FullName;
            Links = new Dictionary<string, string> { ["about"] = ex.HelpLink };
        }
        internal ApiError(HttpError ex)
        {
            Title = ex.ExceptionMessage;
            Detail = ex.StackTrace;
            Code = ex.ExceptionType;
        }

        public string Title { get; }
        public string Detail { get; }
        public string Code { get; }
        public Dictionary<string, string> Links { get; }
    }
}
