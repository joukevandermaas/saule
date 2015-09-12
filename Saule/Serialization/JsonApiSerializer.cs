using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Saule.Serialization
{
    internal class JsonApiSerializer
    {
        public JObject Serialize(ApiResponse response)
        {
            var objectJson = JObject.FromObject(response.Object);
            var data = new JObject();
            var resource = response.Resource;
            data["type"] = resource.ResourceType.ToDashed();
            data["id"] = ResolveAttributeValue("id", objectJson);

            data["attributes"] = GetAttributes(response.Resource, objectJson);

            return new JObject { { "data", data } };
        }

        private JToken GetAttributes(ApiResource resource, IDictionary<string, JToken> properties)
        {
            var attributes = new JObject();
            foreach (var attr in resource.Attributes)
            {
                attributes.Add(attr.Name, ResolveAttributeValue(attr.Name, properties));
            }

            return attributes;
        }

        private JToken ResolveAttributeValue(string name, IDictionary<string, JToken> properties)
        {
            return properties[name.ToPascalCase()];
        }
    }
}
