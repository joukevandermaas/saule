using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Saule.Serialization
{
    internal class ApiError
    {
        private readonly JsonApiException _exception;

        public ApiError()
        {
        }

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

        public string Title { get; internal set; }

        public object Detail { get; internal set; }

        public string Code { get; }

        public Dictionary<string, string> Links { get; }

        public static bool IsClientError(ApiError error)
        {
            return error._exception != null && error._exception.ErrorType == ErrorType.Client;
        }

        public static bool AreClientErrors(ApiError[] errors)
        {
            return errors
                .Min(e => e._exception != null ? e._exception.ErrorType : ErrorType.Server) == ErrorType.Client;
        }
    }
}