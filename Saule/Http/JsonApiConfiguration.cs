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
        /// Initializes a new instance of the <see cref="JsonApiConfiguration"/> class.
        /// </summary>
        public JsonApiConfiguration()
        {
        }

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
    }
}