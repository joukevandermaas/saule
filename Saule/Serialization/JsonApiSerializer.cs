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
                (properties, multiple) => SerializeData(
                    resource,
                    multiple
                        ? CombineUris(baseUrl, GetValue("id", properties).ToString())
                        : baseUrl,
                    properties,
                    included));

            result["included"] = included;

            return result;
        }

        private JToken SerializeArrayOrObject(JToken token, Func<IDictionary<string, JToken>, JToken> SerializeObj)
        {
            return SerializeArrayOrObject(token, (s, b) => SerializeObj(s));
        }

        private JToken SerializeArrayOrObject(JToken token, Func<IDictionary<string, JToken>, bool, JToken> SerializeObj)
        {
            var dataArray = token as JArray;
            if (dataArray != null)
            {
                var data = new JArray();
                foreach (var obj in dataArray)
                {
                    if (obj is JObject)
                        data.Add(SerializeObj(obj as JObject, true));
                }
                return data;
            }
            else
            {
                return token is JObject ? SerializeObj(token as JObject, false) : null;
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
                        props =>
                        {
                            var values = SerializeMinimalData(rel.RelatedResource, props);
                            var includedData = values.DeepClone();
                            includedData["attributes"] = SerializeAttributes(rel.RelatedResource, props);
                            if (!ContainsResource(included, includedData))
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

        private bool ContainsResource(JArray included, JToken includedData)
        {
            return included.Any(t =>
                t.Value<string>("type") == t.Value<string>("type") &&
                t.Value<string>("id") == t.Value<string>("id"));
        }

        private JToken EnsureHasId(IDictionary<string, JToken> properties)
        {
            var id = GetValue("id", properties);
            if (id == null) throw new JsonApiException("Resources must have an id");

            return id;
        }

        private string CombineUris(params string[] parts)
        {
            var result = string.Join("/", parts.Select(s => s.Trim('/')).ToArray());
            return Uri.IsWellFormedUriString(parts[0], UriKind.Absolute)
                ? result
                : "/" + result;
        }
    }
}