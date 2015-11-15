using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public JToken Serialize(object @object, ApiResource resource, Uri requestUri)
        {
            try
            {
                if (requestUri == null)
                {
                    throw new ArgumentNullException(nameof(requestUri));
                }

                var error = SerializeAsError(@object);
                if (error != null)
                {
                    return error;
                }

                var dataObject = @object;
                if (QueryContext?.Sorting != null)
                {
                    dataObject = Query.ApplySorting(dataObject, QueryContext.Sorting);
                }

                if (QueryContext?.Pagination != null)
                {
                    dataObject = Query.ApplyPagination(dataObject, QueryContext.Pagination);
                }

                var serializer = new ResourceSerializer(
                    value: dataObject,
                    type: resource,
                    baseUrl: requestUri,
                    urlBuilder: UrlPathBuilder,
                    paginationContext: QueryContext?.Pagination);

                var jsonSerializer = GetJsonSerializer();
                return serializer.Serialize(jsonSerializer);
            }
            catch (JsonApiException ex)
            {
                return SerializeAsError(ex);
            }
        }

        private static JToken SerializeAsError(object @object)
        {
            var exception = @object as Exception;
            if (exception != null)
            {
                var error = new ApiError(exception);
                return new ErrorSerializer().Serialize(error);
            }

            var httpError = @object as HttpError;
            if (httpError != null)
            {
                var error = new ApiError(httpError);
                return new ErrorSerializer().Serialize(error);
            }

            return null;
        }

        private JsonSerializer GetJsonSerializer()
        {
            var serializer = new JsonSerializer();
            foreach (var converter in JsonConverters)
            {
                serializer.Converters.Add(converter);
            }

            return serializer;
        }
    }
}
