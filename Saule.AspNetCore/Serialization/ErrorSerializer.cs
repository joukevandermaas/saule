using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ErrorSerializer
    {
        public JObject Serialize(ApiError[] errors)
        {
            var results = new JArray();

            foreach (var error in errors)
            {
                var result = new JObject();

                var json = JObject.FromObject(error, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

                foreach (var token in json)
                {
                    result.Add(token.Key.ToCamelCase(), token.Value);
                }

                results.Add(result);
            }

            return new JObject { ["errors"] = results };
        }
    }
}
