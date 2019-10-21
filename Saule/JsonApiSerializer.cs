using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Http;
using Saule.Queries;
using Saule.Queries.Pagination;
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

        public PreprocessResult PreprocessContent(object @object, ApiResource resource, Uri requestUri, JsonApiConfiguration config)
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
                if (QueryContext != null && QueryContext.Pagination != null)
                {
                    // weare trying to extract TotalResultsCount from the dataObject
                    dataObject = new PagedResultQuery(QueryContext.Pagination).Apply(dataObject);
                }

                if (QueryContext != null && !QueryContext.IsHandledQuery)
                {
                    if (QueryContext.Filter != null)
                    {
                        dataObject = Query.ApplyFiltering(dataObject, QueryContext.Filter, resource);
                    }

                    if (QueryContext.Sort != null)
                    {
                        dataObject = Query.ApplySorting(dataObject, QueryContext.Sort, resource);
                    }

                    if (QueryContext.Pagination != null)
                    {
                        dataObject = Query.ApplyPagination(dataObject, QueryContext.Pagination, resource);
                    }
                }

                result.ResourceSerializer = new ResourceSerializer(
                        value: dataObject,
                        type: resource,
                        baseUrl: requestUri,
                        propertyNameConverter: config.PropertyNameConverter,
                        urlBuilder: UrlPathBuilder,
                        paginationContext: QueryContext?.Pagination,
                        includeContext: QueryContext?.Include,
                        fieldsetContext: QueryContext?.Fieldset);
            }
            catch (Exception ex)
            {
                result.ErrorContent = GetAsError(ex);
            }

            return result;
        }

        private static List<ApiError> GetAsError(object @object)
        {
            var exception = @object as Exception;
            if (exception != null)
            {
                return new List<ApiError>() { new ApiError(exception) };
            }

            var httpError = @object as HttpError;
            if (httpError != null)
            {
                return new List<ApiError>() { new ApiError(httpError) };
            }

            var httpErrorList = @object as IEnumerable<HttpError>;
            if (httpErrorList != null)
            {
                return httpErrorList.Select(error => new ApiError(error)).ToList();
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