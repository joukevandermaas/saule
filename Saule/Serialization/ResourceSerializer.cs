﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Saule.Queries;

namespace Saule.Serialization
{
    internal class ResourceSerializer
    {
        private readonly Uri _baseUrl;
        private readonly PaginationContext _paginationContext;
        private readonly ApiResource _resource;
        private readonly object _value;
        private readonly JArray _includedSection;
        private bool _isCollection;
        private readonly IUrlPathBuilder _urlBuilder;
        private readonly string _commonPathSpec;

        public ResourceSerializer(
            object value,
            ApiResource type,
            Uri baseUrl,
            IUrlPathBuilder urlBuilder,
            PaginationContext paginationContext)
        {
            _urlBuilder = urlBuilder;
            _resource = type;
            _value = value;
            _baseUrl = baseUrl;
            _paginationContext = paginationContext;
            _includedSection = new JArray();

            var firstPart = _urlBuilder.BuildCanonicalPath(_resource)
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (firstPart != null)
            {
                var existingPath = _baseUrl.AbsolutePath;
                var startIndex = existingPath.IndexOf(firstPart, StringComparison.InvariantCulture);
                var genericPart = startIndex > -1
                    ? existingPath.Substring(0, startIndex)
                    : existingPath;

                _commonPathSpec = genericPart;
            }
            else
            {
                _commonPathSpec = "/";
            }
        }

        public JObject Serialize()
        {
            return Serialize(new JsonSerializer());
        }

        public JObject Serialize(JsonSerializer serializer)
        {
            if (_value == null) return SerializeNull(serializer);
            var objectJson = JToken.FromObject(_value, serializer);
            _isCollection = objectJson is JArray;

            var result = new JObject
            {
                ["data"] = SerializeArrayOrObject(objectJson, SerializeData),
                ["links"] = new JObject
                {
                    ["self"] = new JValue(_baseUrl)
                }
                ["links"] = CreateTopLevelLinks(_isCollection ? objectJson.Count() : 0)
            };

            if (_includedSection.Count > 0) result["included"] = _includedSection;

            return result;
        }

        private JObject SerializeNull(JsonSerializer serializer)
        {
            return new JObject
            {
                ["data"] = null,
                ["links"] = CreateTopLevelLinks(0)
            };
        }

        private JToken CreateTopLevelLinks(int count)
        {
            var result = new JObject
            {
                ["self"] = _baseUrl
            };

            var queryStrings = new PaginationQuery(_paginationContext);

            var left = _baseUrl.GetLeftPart(UriPartial.Path);

            if (queryStrings.FirstPage != null)
                result["first"] = new Uri(left + queryStrings.FirstPage);

            if (queryStrings.NextPage != null && count >= _paginationContext.PerPage)
                result["next"] = new Uri(left + queryStrings.NextPage);

            if (queryStrings.PreviousPage != null)
                result["prev"] = new Uri(left + queryStrings.PreviousPage);

            return result;
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
                data["links"] = AddUrl(new JObject(), "self",
                    _urlBuilder.BuildCanonicalPath(_resource, EnsureHasId(properties).Value<string>()));
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
            var relationshipValues = GetValue(relationship.Name, properties);
            var relationshipProperties = relationshipValues as JObject;

            // serialize the links part (so the data can be fetched)
            var objId = EnsureHasId(properties).Value<string>();
            var relToken = GetMinimumRelationship(objId, relationship, 
                relationshipProperties != null ? GetValue("id", relationshipProperties).Value<string>() : null);
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
                    includedData["links"] = AddUrl(new JObject(), "self",
                        _urlBuilder.BuildCanonicalPath(relationship.RelatedResource, EnsureHasId(props).Value<string>()));
                    if (!IsResourceIncluded(includedData))
                        _includedSection.Add(includedData);

                    return values;
                });
            return data;
        }

        private JToken GetMinimumRelationship(string id, ResourceRelationship relationship, string relationshipId)
        {
            var links = new JObject();
            AddUrl(links, "self", _urlBuilder.BuildRelationshipSelfPath(_resource, id, relationship, relationshipId));
            AddUrl(links, "related", _urlBuilder.BuildRelationshipPath(_resource, id, relationship));

            return new JObject
            {
                ["links"] = links
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

        private JObject AddUrl(JObject @object, string name, string path)
        {
            if (string.IsNullOrEmpty(path)) return @object;

            var start = new Uri(_baseUrl.GetLeftPart(UriPartial.Authority).EnsureEndsWith("/"));
            string combined;
            if (!string.IsNullOrEmpty(path) && path[0] == '/')
                combined = path.EnsureEndsWith("/");
            else
                combined = '/'.TrimJoin(_commonPathSpec, path).EnsureEndsWith("/");

            @object.Add(name, new Uri(start, combined));

            return @object;
        }
    }
}