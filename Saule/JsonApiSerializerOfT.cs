using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
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
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name must match first type name",
        Justification = "Non-generic version exists")]
    public sealed class JsonApiSerializer<T>
            where T : ApiResource, new()
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
        /// Contains converters to influence the serialization process.
        /// </summary>
        public ICollection<JsonConverter> JsonConverters => _serializer.JsonConverters;

        /// <summary>
        /// True if responses should be paginated, otherwise false.
        /// </summary>
        public bool Paginate { get; set; } = false;

        /// <summary>
        /// True if users are allowed to query this response, otherwise false.
        /// </summary>
        public bool AllowUserQuery
        {
            get { return _serializer.AllowUserQuery; }
            set { _serializer.AllowUserQuery = value; }
        }

        /// <summary>
        /// The number of items per page, if the responses are paginated.
        /// </summary>
        public int ItemsPerPage { get; set; } = 10;

        /// <summary>
        /// The url path builder to use during serialization.
        /// </summary>
        public IUrlPathBuilder UrlPathBuilder
        {
            get { return _serializer.UrlPathBuilder; }
            set { _serializer.UrlPathBuilder = value; }
        }

        /// <summary>
        /// Produces the Json Api response that represents the given @object.
        /// </summary>
        /// <param name="object">The object to serialize.</param>
        /// <param name="requestUri">The request uri that prompted the response.</param>
        /// <returns>A <see cref="JToken"/> representing the object.</returns>
        public JToken Serialize(object @object, Uri requestUri)
        {
            if (!Paginate)
            {
                return _serializer.Serialize(@object, new T(), requestUri);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var context = new PaginationContext(request.GetQueryNameValuePairs(), ItemsPerPage);
            _serializer.QueryContext = new QueryContext { Pagination = context };

            return _serializer.Serialize(@object, new T(), requestUri);
        }
    }
}
