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
            var objectJson = JToken.FromObject(response.Object);
            var resource = response.Resource;
            var result = new JObject();

            var included = new JArray();

            result["data"] = SerializeArrayOrObject(objectJson, 
                properties => SerializeData(resource, baseUrl, properties, included));

            result["included"] = included;

            return result;
        }

        private JToken SerializeArrayOrObject(JToken token, Func<IDictionary<string, JToken>, JToken> SerializeObj)
        {
            var dataArray = token as JArray;
            if (dataArray != null)
            {
                var data = new JArray();
                foreach (var obj in dataArray)
                {
                    if (obj is JObject)
                        data.Add(SerializeObj(obj as JObject));
                }
                return data;
            }
            else
            {
                return token is JObject ? SerializeObj(token as JObject) : null;
            }
        }

        private JToken SerializeData(ApiResource resource, string baseUrl, IDictionary<string, JToken> properties, JArray included)
        {
            var data = SerializeMinimalData(resource, properties);

            data["attributes"] = SerializeAttributes(resource, properties);
            data["relationships"] = SerializeRelationships(resource, baseUrl, properties, included);

            return data;
        }
        private JToken SerializeMinimalData(ApiResource resource, IDictionary<string, JToken> properties)
        {
            var data = new JObject();
            data["type"] = resource.ResourceType.ToDashed();
            data["id"] = EnsureHasId(properties);

            return data;
        }

        private JToken SerializeAttributes(ApiResource resource, IDictionary<string, JToken> properties)
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

        private JToken SerializeRelationships(ApiResource resource, string baseUrl, IDictionary<string, JToken> properties, JArray included)
        {
            var relationships = new JObject();

            foreach (var rel in resource.Relationships)
            {
                var relToken = new JObject();
                relToken["links"] = new JObject {
                    { "self",  CombineUris(baseUrl, "relationships", rel.UrlPath) },
                    { "related",  CombineUris(baseUrl, rel.UrlPath) }
                };
                var relationshipValues = GetValue(rel.Name, properties);
                if (relationshipValues != null)
                {
                    var data = SerializeArrayOrObject(relationshipValues, 
                        props => {
                            var values = SerializeMinimalData(rel.RelatedResource, props);
                            var includedData = values.DeepClone();
                            includedData["attributes"] = SerializeAttributes(rel.RelatedResource, props);
                            included.Add(includedData);

                            return values;
                        });
                    if (data != null)
                        relToken["data"] = data;
                }
                relationships[rel.Name] = relToken;
            }

            return relationships;
        }

        private JToken EnsureHasId(IDictionary<string, JToken> properties)
        {
            var id = GetValue("id", properties);
            if (id == null) throw new JsonApiException("Resources must have an id");

            return id;
        }

        private string CombineUris(params string[] parts)
        {
            return "/" + string.Join("/", parts.Select(s => s.Trim('/')).ToArray());
        }
    }
}
