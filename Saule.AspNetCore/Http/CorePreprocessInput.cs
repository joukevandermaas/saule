using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Saule.Queries;
using Saule.Serialization;

namespace Saule.Http
{
    internal class CorePreprocessInput : PreprocessInput
    {
        private readonly ActionExecutedContext _context;

        public CorePreprocessInput(ActionExecutedContext context)
        {
            _context = context;
        }

        public override ApiResource ResourceDescriptor
        {
            get
            {
                if (_context.HttpContext.Items.ContainsKey(Constants.PropertyNames.ResourceDescriptor))
                {
                    return (ApiResource)_context.HttpContext.Items[Constants.PropertyNames.ResourceDescriptor];
                }

                return null;
            }
        }

        public override object Content => (_context.Result as ObjectResult)?.Value;

        public override IEnumerable<ApiError> ErrorContent
        {
            get
            {
                if (_context.Exception != null)
                {
                    yield return new ApiError(_context.Exception);
                }

                if ((_context.Result as ObjectResult)?.Value is SerializableError errors)
                {
                    foreach (var error in errors)
                    {
                        if (error.Key == "Request content" && error.Value is IEnumerable<string> messages)
                        {
                            foreach (var message in messages)
                            {
                                yield return new ApiError(new JsonApiException(ErrorType.Client, message));
                            }
                        }
                        else
                        {
                            yield return new ApiError(error.Value.ToString(), detail: null, code: null);
                        }
                    }
                }
            }
        }

        public override int StatusCode => _context.HttpContext.Response.StatusCode;

        public override Uri RequestUri => new Uri(_context.HttpContext.Request.GetEncodedUrl());

        public override string RouteTemplate => _context.ActionDescriptor.AttributeRouteInfo.Template;

        public override string VirtualPathRoot => _context.HttpContext.Request.PathBase.Value;

        public override QueryContext QueryContext => (QueryContext)_context.HttpContext.Items[Constants.PropertyNames.QueryContext];

        public override IEnumerable<string> AcceptMediaTypes
        {
            get
            {
                var headers = _context.HttpContext.Request.Headers;
                if (headers.ContainsKey("Accept"))
                {
                    return headers["Accept"];
                }
                return Enumerable.Empty<string>();
            }
        }
    }
}