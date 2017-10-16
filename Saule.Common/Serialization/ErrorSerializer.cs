using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ErrorSerializer
    {
        public JObject Serialize(ApiError[] errors)
        {
            return new JObject
            {
                ["errors"] = new JArray(errors.Select((error) =>
                {
                    var result = new JObject();

                    var json = JObject.FromObject(
                        error,
                        new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

                    foreach (var token in json)
                    {
                        result.Add(token.Key.ToCamelCase(), token.Value);
                    }

                    return result;
                }))
            };
        }
    }
}