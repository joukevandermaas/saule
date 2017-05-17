using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Http;
using Saule.Queries;
using Saule.Serialization;

namespace Saule
{
    internal sealed class JsonApiSerializer
    {
        public List<JsonConverter> JsonConverters { get; } = new List<JsonConverter>();

        public QueryContext QueryContext { get; set; } = null;

        public IUrlPathBuilder UrlPathBuilder { get; set; } = new DefaultUrlPathBuilder();

        public bool AllowUserQuery { get; set; } = false;

        public static JToken Serialize(PreprocessResult result)
        {
            if (result.ErrorContent != null)
            {
                return new ErrorSerializer().Serialize(result.ErrorContent);
            }

            var jsonSerializer = GetJsonSerializer(result.JsonConverters);
            return result.ResourceSerializer.Serialize(jsonSerializer);
        }

        public PreprocessResult PreprocessContent(object @object, ApiResource resource, Uri requestUri)
        {
            var result = new PreprocessResult
            {
                JsonConverters = JsonConverters
            };
            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            try
            {
                var error = GetAsError(@object);
                if (error != null)
                {
                    result.ErrorContent = error;
                    return result;
                }

                var dataObject = @object;

                if (QueryContext?.Filtering != null)
                {
                    dataObject = Query.ApplyFiltering(dataObject, QueryContext.Filtering, resource);
                }

                if (QueryContext?.Sorting != null)
                {
                    dataObject = Query.ApplySorting(dataObject, QueryContext.Sorting, resource);
                }

                if (QueryContext?.Pagination != null)
                {
                    dataObject = Query.ApplyPagination(dataObject, QueryContext.Pagination, resource);
                }

                result.ResourceSerializer = new ResourceSerializer(
                        value: dataObject,
                        type: resource,
                        baseUrl: requestUri,
                        urlBuilder: UrlPathBuilder,
                        paginationContext: QueryContext?.Pagination,
                        includingContext: QueryContext?.Including);
            }
            catch (Exception ex)
            {
                result.ErrorContent = GetAsError(ex);
            }

            return result;
        }

        private static ApiError GetAsError(object @object)
        {
            var exception = @object as Exception;
            if (exception != null)
            {
                return new ApiError(exception);
            }

            var httpError = @object as HttpError;
            if (httpError != null)
            {
                return new ApiError(httpError);
            }

            return null;
        }

        private static JsonSerializer GetJsonSerializer(IEnumerable<JsonConverter> converters)
        {
            var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            foreach (var converter in converters)
            {
                serializer.Converters.Add(converter);
            }

            return serializer;
        }
    }
}