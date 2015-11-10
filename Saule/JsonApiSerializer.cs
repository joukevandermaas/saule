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
    internal sealed class JsonApiSerializer
    {
        public List<JsonConverter> JsonConverters { get; } = new List<JsonConverter>();
        public PaginationContext PaginationContext { get; set; } = null;
        public IUrlPathBuilder UrlPathBuilder { get; set; } = new DefaultUrlPathBuilder();

        public JToken Serialize(object @object, ApiResource resource, Uri requestUri)
        {
            try
            {
                if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

                var error = SerializeAsError(@object);
                if (error != null) return error;

                var dataObject = @object;
                if (PaginationContext != null)
                {
                    dataObject = PaginationInterpreter.ApplyPaginationIfApplicable(PaginationContext, dataObject);
                }

                var serializer = new ResourceSerializer(dataObject, resource, requestUri, UrlPathBuilder, PaginationContext);
                var jsonSerializer = GetJsonSerializer();
                return serializer.Serialize(jsonSerializer);
            }
            catch (JsonApiException ex)
            {
                return SerializeAsError(ex);
            }
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
    }

    /// <summary>
    /// Used to manually serialize objects into Json Api.
    /// </summary>
    /// <typeparam name="T">The resource type of the objects this serializer can serialize.</typeparam>
    public sealed class JsonApiSerializer<T> where T : ApiResource, new()
    {
        private readonly JsonApiSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiSerializer{T}"/> class.
        /// </summary>
        public JsonApiSerializer()
        {
            _serializer = new JsonApiSerializer();
        }

        /// <summary>
        /// Produces the Json Api response that represents the given @object.
        /// </summary>
        /// <param name="object">The object to serialize.</param>
        /// <param name="requestUri">The request uri that prompted the response.</param>
        /// <returns></returns>
        public JToken Serialize(object @object, Uri requestUri)
        {
            if (!Paginate) return _serializer.Serialize(@object, new T(), requestUri);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var context = new PaginationContext(request.GetQueryNameValuePairs(), ItemsPerPage);
            _serializer.PaginationContext = context;

            return _serializer.Serialize(@object, new T(), requestUri);
        }

        /// <summary>
        /// Contains converters to influence the serialization process.
        /// </summary>
        public ICollection<JsonConverter> JsonConverters => _serializer.JsonConverters;

        /// <summary>
        /// True if responses should be paginated, otherwise false.
        /// </summary>
        public bool Paginate { get; set; }

        /// <summary>
        /// The number of items per page, if the responses are paginated.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// The url path builder to use during serialization.
        /// </summary>
        public IUrlPathBuilder UrlPathBuilder
        {
            get { return _serializer.UrlPathBuilder; }
            set { _serializer.UrlPathBuilder = value; }
        }
    }
}
