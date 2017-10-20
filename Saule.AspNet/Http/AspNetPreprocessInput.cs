using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Saule.Queries;
using Saule.Serialization;

namespace Saule.Http
{
    internal class AspNetPreprocessInput : PreprocessInput
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpResponseMessage _actionMethodResult;
        private readonly List<ApiError> _errors;

        public AspNetPreprocessInput(HttpRequestMessage request, HttpResponseMessage actionMethodResult)
        {
            _request = request;
            _actionMethodResult = actionMethodResult;
            _errors = new List<ApiError>();
        }

        public void AddError(Exception ex)
        {
            _errors.Add(new ApiError(ex));
        }

        public override object Content => (_actionMethodResult.Content as ObjectContent)?.Value;

        public override IEnumerable<ApiError> ErrorContent
        {
            get
            {
                if (Content is Exception ex)
                {
                    return _errors.Concat(new[] { new ApiError(ex) });
                }

                if (Content is IEnumerable<Exception> exs)
                {
                    return _errors.Concat(exs.Select(e => new ApiError(e)));
                }

                if (Content is HttpError err)
                {
                    return _errors.Concat(new[] { new ApiError(err.Message, err.MessageDetail, err.ExceptionType) });
                }

                return _errors;
            }
        }

        public override ApiResource ResourceDescriptor
        {
            get
            {
                if (_request.Properties.ContainsKey(Constants.PropertyNames.ResourceDescriptor))
                {
                    return (ApiResource)_request.Properties[Constants.PropertyNames.ResourceDescriptor];
                }
                return null;
            }
        }

        public override int StatusCode => (int)_actionMethodResult.StatusCode;

        public override Uri RequestUri => _request.RequestUri;

        public override string RouteTemplate
        {
            get
            {
                var requestContext = _request.Properties[Constants.PropertyNames.WebApiRequestContext]
                    as HttpRequestContext;
                return requestContext?.RouteData.Route.RouteTemplate;
            }
        }

        public override string VirtualPathRoot
        {
            get
            {
                var requestContext = _request.Properties[Constants.PropertyNames.WebApiRequestContext]
                    as HttpRequestContext;
                return requestContext?.VirtualPathRoot;
            }
        }

        public override QueryContext QueryContext
        {
            get
            {
                if (!_request.Properties.ContainsKey(Constants.PropertyNames.QueryContext))
                {
                    return null;
                }

                return (QueryContext)_request.Properties[Constants.PropertyNames.QueryContext];
            }
        }

        public override IEnumerable<string> AcceptMediaTypes => _request.Headers.Accept.Select(s => s.MediaType);
    }
}