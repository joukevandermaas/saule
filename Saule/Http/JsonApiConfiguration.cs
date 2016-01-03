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
        /// Determines how to generate urls for links.
        /// </summary>
        public IUrlPathBuilder UrlPathBuilder { get; set; } = new DefaultUrlPathBuilder();

        /// <summary>
        /// Json converters to manipulate the serialization process.
        /// </summary>
        public List<JsonConverter> JsonConverters { get; } = new List<JsonConverter>();

        /// <summary>
        /// Determines the expressions that are used to evaluate filter queries on a per-type basis.
        /// </summary>
        public QueryFilterExpressionCollection QueryFilterExpressions { get; } = new QueryFilterExpressionCollection();
    }
}