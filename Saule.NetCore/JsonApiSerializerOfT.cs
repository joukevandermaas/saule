using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Http;
using Saule.Queries;
using Saule.Queries.Filtering;
using Saule.Queries.Including;
using Saule.Queries.Pagination;
using Saule.Queries.Sorting;
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
        /// Gets the converters to influence the serialization process.
        /// </summary>
        public ICollection<JsonConverter> JsonConverters => _serializer.JsonConverters;

        /// <summary>
        /// Gets or sets a value indicating whether responses should be paginated.
        /// </summary>
        public bool Paginate { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to query this response.
        /// </summary>
        public bool AllowQuery
        {
            get { return _serializer.AllowUserQuery; }
            set { _serializer.AllowUserQuery = value; }
        }

        /// <summary>
        /// Gets the expressions used to execute filters specified through query parameters in the request url.
        /// </summary>
        public QueryFilterExpressionCollection QueryFilterExpressions { get; } = new QueryFilterExpressionCollection();

        /// <summary>
        /// Gets or sets the number of items per page, if the responses are paginated.
        /// </summary>
        public int ItemsPerPage { get; set; } = 10;

        /// <summary>
        /// Gets or sets the url path builder to use during serialization.
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
            var queryContext = GetQueryContext(requestUri.GetQueryNameValuePairs());

            _serializer.QueryContext = queryContext;

            var preprocessResult = _serializer.PreprocessContent(@object, new T(), requestUri);
            return JsonApiSerializer.Serialize(preprocessResult);
        }

        private QueryContext GetQueryContext(IEnumerable<KeyValuePair<string, string>> filters)
        {
            var context = new QueryContext();
            var keyValuePairs = filters as IList<KeyValuePair<string, string>> ?? filters.ToList();

            if (Paginate)
            {
                context.Pagination = new PaginationContext(keyValuePairs, ItemsPerPage);
            }

            if (AllowQuery)
            {
                context.Sorting = new SortingContext(keyValuePairs);
                context.Filtering = new FilteringContext(keyValuePairs) { QueryFilters = QueryFilterExpressions };
                context.Including = new IncludingContext(keyValuePairs);
            }

            return context;
        }
    }
}
