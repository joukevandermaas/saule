using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saule.Serialization
{
    internal class ResourceSerializer
    {
        private readonly Uri _baseUrl;
        private readonly ApiResource _resource;
        private readonly object _value;
        private readonly JArray _includedSection;
        private bool _isCollection;

        public ResourceSerializer(object value, ApiResource type, Uri baseUrl)
        {
            _resource = type;
            _value = value;
            _baseUrl = baseUrl;
            _includedSection = new JArray();
        }

        public JObject Serialize()
        {
            var objectJson = JToken.FromObject(_value);
            _isCollection = objectJson is JArray;

            return new JObject
            {
                ["data"] = SerializeArrayOrObject(objectJson, SerializeData),
                ["included"] = _includedSection,
                ["links"] = new JObject
                {
                    ["self"] = new JValue(_baseUrl)
                }
            };
        }

        private static JToken SerializeArrayOrObject(JToken token, Func<IDictionary<string, JToken>, JToken> serializeObj)
        {
            var dataArray = token as JArray;

            // single thing, just serialize it
            if (dataArray == null) return token is JObject ? serializeObj((JObject)token) : null;

            // serialize each element separately
            var data = new JArray();
            foreach (var obj in dataArray.OfType<JObject>())
            {
                data.Add(serializeObj(obj));
            }
            return data;
        }

        private JToken SerializeData(IDictionary<string, JToken> properties)
        {
            var data = SerializeMinimalData(properties);

            data["attributes"] = SerializeAttributes(properties);
            data["relationships"] = SerializeRelationships(properties);

            if (_isCollection)
            {
                data["links"] = new JObject
                {
                    ["self"] = GetUrl(EnsureHasId(properties).Value<string>())
                };
            }

            return data;
        }

        private JToken SerializeMinimalData(IDictionary<string, JToken> properties)
        {
            return SerializeMinimalData(properties, _resource);
        }
        private static JToken SerializeMinimalData(IDictionary<string, JToken> properties, ApiResource resource)
        {
            var data = new JObject
            {
                ["type"] = resource.ResourceType.ToDashed(),
                ["id"] = EnsureHasId(properties)
            };

            return data;
        }

        private JToken SerializeAttributes(IDictionary<string, JToken> properties)
        {
            return SerializeAttributes(properties, _resource);
        }
        private static JToken SerializeAttributes(IDictionary<string, JToken> properties, ApiResource resource)
        {
            var attributes = new JObject();
            foreach (var attr in resource.Attributes)
            {
                attributes.Add(attr.Name, GetValue(attr.Name, properties));
            }

            return attributes;
        }

        private JToken SerializeRelationships(IDictionary<string, JToken> properties)
        {
            var relationships = new JObject();

            foreach (var rel in _resource.Relationships)
            {
                relationships[rel.Name] = SerializeRelationship(rel, properties);
            }

            return relationships;
        }

        private JToken SerializeRelationship(ResourceRelationship relationship, IDictionary<string, JToken> properties)
        {
            // serialize the links part (so the data can be fetched)
            var objId = _isCollection 
                ? EnsureHasId(properties).Value<string>()
                : string.Empty;
            var relToken = GetMinimumRelationship(objId, relationship.UrlPath);
            var relationshipValues = GetValue(relationship.Name, properties);
            if (relationshipValues == null) return relToken;

            // only include data if it exists, otherwise just assume it should be fetched later
            var data = GetRelationshipData(relationship, relationshipValues);
            if (data != null)
                relToken["data"] = data;

            return relToken;
        }

        private JToken GetRelationshipData(ResourceRelationship relationship, JToken relationshipValues)
        {
            var data = SerializeArrayOrObject(relationshipValues,
                props =>
                {
                    var values = SerializeMinimalData(props, relationship.RelatedResource);
                    var includedData = values.DeepClone();
                    includedData["attributes"] = SerializeAttributes(props, relationship.RelatedResource);
                    if (!IsResourceIncluded(includedData))
                        _includedSection.Add(includedData);

                    return values;
                });
            return data;
        }

        private JToken GetMinimumRelationship(string id, string urlPath)
        {
            return new JObject
            {
                ["links"] = new JObject
                {
                    ["self"] = GetUrl(id, "relationships", urlPath),
                    ["related"] = GetUrl(id, urlPath)
                }
            };
        }

        private bool IsResourceIncluded(JToken includedData)
        {
            return _includedSection.Any(t =>
                t.Value<string>("type") == includedData.Value<string>("type") &&
                t.Value<string>("id") == includedData.Value<string>("id"));
        }
        private static JToken GetValue(string name, IDictionary<string, JToken> properties)
        {
            return properties[name.ToPascalCase()];
        }
        private static JToken EnsureHasId(IDictionary<string, JToken> properties)
        {
            var id = GetValue("id", properties);
            if (id == null) throw new JsonApiException("Resources must have an id");

            return id;
        }
        private Uri GetUrl(params string[] parts)
        {
            var path = new Uri(_baseUrl.GetLeftPart(UriPartial.Path).EnsureEndsWith("/"));

            var goodParts = parts.Where(s => !string.IsNullOrEmpty(s));
            var result = string.Join("/", goodParts.Select(s => s.Trim('/')).ToArray()) + "/";

            return new Uri(path, result);
        }
    }
}