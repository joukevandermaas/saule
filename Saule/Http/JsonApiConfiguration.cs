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
        public ICollection<JsonConverter> JsonConverters { get; } = new List<JsonConverter>();
    }
}