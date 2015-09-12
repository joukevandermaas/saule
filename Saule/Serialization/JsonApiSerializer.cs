using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saule.Serialization
{
    internal class JsonApiSerializer
    {
        public JObject Serialize(ApiResponse response, string baseUrl)
        {
            var objectJson = JObject.FromObject(response.Object);
            var resource = response.Resource;
            var data = GetData(resource, baseUrl, objectJson);

            var result = new JObject();

            result["data"] = data;

            return result;
        }

        private JToken GetData(ApiResource resource, string baseUrl, IDictionary<string, JToken> properties)
        {
            var data = new JObject();
            data["type"] = resource.ResourceType.ToDashed();
            data["id"] = GetValue("id", properties);

            data["attributes"] = GetAttributes(resource, properties);
            data["relationships"] = GetRelationships(resource, baseUrl, properties);

            return data;
        }

        private JToken GetAttributes(ApiResource resource, IDictionary<string, JToken> properties)
        {
            var attributes = new JObject();
            foreach (var attr in resource.Attributes)
            {
                attributes.Add(attr.Name, GetValue(attr.Name, properties));
            }

            return attributes;
        }
        private JToken GetValue(string name, IDictionary<string, JToken> properties)
        {
            return properties[name.ToPascalCase()];
        }

        private JToken GetRelationships(ApiResource resource, string baseUrl, IDictionary<string, JToken> properties)
        {
            var relationships = new JObject();

            foreach (var rel in resource.Relationships)
            {
                var relToken = new JObject();
                relToken["links"] = new JObject {
                    { "self",  CombineUris(baseUrl, "relationships", rel.UrlPath) },
                    { "related",  CombineUris(baseUrl, rel.UrlPath) }
                };
                relationships[rel.Name] = relToken;
            }

            return relationships;
        }


        private string CombineUris(params string[] parts)
        {
            return "/" + string.Join("/", parts.Select(s => s.Trim('/')).ToArray());
        }
    }
}
