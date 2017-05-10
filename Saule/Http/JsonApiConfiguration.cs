using System.Collections.Generic;
using Newtonsoft.Json;
using Saule.Serialization;

namespace Saule.Http
{
    /// <summary>
    /// Contains settings to influence the Json Api serialization process.
    /// </summary>
    public class JsonApiConfiguration
    {
        /// <summary>
        /// Gets or sets the UrlPathBuilder which determines how to generate urls for links.
        /// </summary>
        public IUrlPathBuilder UrlPathBuilder { get; set; } = null;

        /// <summary>
        /// Gets the JsonConverters to manipulate the serialization process.
        /// </summary>
        public List<JsonConverter> JsonConverters { get; } = new List<JsonConverter>();

        /// <summary>
        /// Gets the expressions that are used to evaluate filter queries on a per-type basis.
        /// </summary>
        public QueryFilterExpressionCollection QueryFilterExpressions { get; } = new QueryFilterExpressionCollection();

        /// <summary>
        /// Gets or sets a value indicating whether the new experimental graph serializer is used.
        /// </summary>
        public bool UseGraphSerializer { get; set; } = false;
    }
}