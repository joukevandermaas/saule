using System.Collections.Generic;
using Newtonsoft.Json;
using Saule.Serialization;

namespace Saule
{
    internal sealed class PreprocessResult
    {
        public ApiError ErrorContent { get; set; }

        public ResourceSerializer ResourceGraphSerializer { get; set; }

        public IEnumerable<JsonConverter> JsonConverters { get; set; }
    }
}