using System;
using System.Collections.Generic;
using System.Linq;

namespace Saule.Serialization
{
    internal class ApiError
    {
        private readonly JsonApiException _exception;

        public ApiError(Exception ex)
        {
            Title = ex.Message;
            Detail = ex.ToString();
            Code = ex.GetType().FullName;
            Links = ex.HelpLink != null
                ? new Dictionary<string, string> { ["about"] = ex.HelpLink }
                : null;

            _exception = ex as JsonApiException;
        }

        public ApiError(string title, string detail, string code)
        {
            Title = title;
            Detail = detail;
            Code = code;
        }

        public string Title { get; }

        public string Detail { get; }

        public string Code { get; }

        public Dictionary<string, string> Links { get; }

        public static bool AnyClientError(params ApiError[] errors)
        {
            return errors.Any(error =>
                error._exception != null && error._exception.ErrorType == ErrorType.Client);
        }
    }
}