using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Queries;
using Saule.Serialization;

namespace Saule
{
    /// <summary>
    /// Used to manually serialize objects into Json Api.
    /// </summary>
    /// <typeparam name="T">The resource type of the objects this serializer can serialize.</typeparam>
    public sealed class JsonApiSerializer<T> where T : ApiResource, new()
    {
        /// <summary>
        /// Produces the Json Api response that represents the given @object.
        /// </summary>
        /// <param name="object">The object to serialize.</param>
        /// <param name="requestUri">The request uri that prompted the response.</param>
        /// <returns></returns>
        public JToken Serialize(object @object, Uri requestUri)
        {
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var error = SerializeAsError(@object);
            if (error != null) return error;

            var dataObject = @object;
            PaginationContext context = null;
            if (Paginate)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                context = new PaginationContext(request.GetQueryNameValuePairs(), ItemsPerPage);
                dataObject = PaginationInterpreter.ApplyPaginationIfApplicable(context, dataObject);
            }

            var serializer = new ResourceSerializer(dataObject, new T(), requestUri, new DefaultUrlPathBuilder(),  context);
            var jsonSerializer = GetJsonSerializer();
            return serializer.Serialize(jsonSerializer);
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

        /// <summary>
        /// Contains converters to influence the serialization process.
        /// </summary>
        public ICollection<JsonConverter> JsonConverters { get; } = new JsonConverterCollection();

        /// <summary>
        /// True if responses should be paginated, otherwise false.
        /// </summary>
        public bool Paginate { get; set; } = false;

        /// <summary>
        /// The number of items per page, if the responses are paginated.
        /// </summary>
        public int ItemsPerPage { get; set; } = 10;
    }
}
