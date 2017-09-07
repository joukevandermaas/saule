using System;
using System.Collections.Generic;
using System.Web.Http;

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

        internal ApiError(HttpError ex)
        {
            Title = GetRecursiveExceptionMessage(ex);
            Detail = ex.StackTrace;
            Code = ex.ExceptionType;
        }

        public string Title { get; }

        public string Detail { get; }

        public string Code { get; }

        public Dictionary<string, string> Links { get; }

        public static bool IsClientError(ApiError error)
        {
            return error._exception != null && error._exception.ErrorType == ErrorType.Client;
        }

        private static string GetRecursiveExceptionMessage(HttpError ex)
        {
            var msg = !string.IsNullOrEmpty(ex.ExceptionMessage) ? ex.ExceptionMessage : ex.Message;
            msg = msg?.EnsureEndsWith(".");
            if (ex.InnerException != null)
            {
                msg += ' ' + GetRecursiveExceptionMessage(ex.InnerException);
            }

            return msg;
        }
    }
}