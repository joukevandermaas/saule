using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ErrorSerializer
    {
        public JObject Serialize(ApiError error)
        {
            var result = error.ToJObject();
            return new JObject { ["errors"] = result };
        }
    }
}
