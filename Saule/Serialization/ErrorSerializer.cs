using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ErrorSerializer
    {
        public JObject Serialize(ApiError error)
        {
            var result = new JObject();

            var json = JObject.FromObject(error, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

            foreach (var token in json)
            {
                result.Add(token.Key.ToCamelCase(), token.Value);
            }

            return new JObject { ["errors"] = new JArray { result } };
        }
    }
}
