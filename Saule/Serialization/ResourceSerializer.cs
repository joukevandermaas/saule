using System;
using System.Collections.Generic;
using System.Linq;
using csharp_extensions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Queries.Pagination;

namespace Saule.Serialization
{
    internal class ResourceSerializer
    {
        private readonly Uri _baseUrl;
        private readonly PaginationContext _paginationContext;
        private readonly ApiResource _resource;
        private readonly object _value;
        private readonly JArray _includedSection;
        private readonly IUrlPathBuilder _urlBuilder;
        private bool _isCollection;

        internal object Value => _value;

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
        }

        public JObject Serialize()
        {
            return Serialize(new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public JObject Serialize(JsonSerializer serializer)
        {
            if (_value == null)
            {
                return SerializeNull();
            }

            //var objectJson = JToken.FromObject(_value, serializer);
            //_isCollection = objectJson is JArray;

            var result = new JObject
            {
                ["data"] = SerializeArrayOrObject(_value, SerializeData),
                ["links"] = new JObject
                {
                    ["self"] = new JValue(_baseUrl)
                }

                //["links"] = CreateTopLevelLinks(_isCollection ? objectJson.Count() : 0)
            };

            if (_includedSection.Count > 0)
            {
                result["included"] = _includedSection;
            }

            return result;
        }

        private static JToken SerializeArrayOrObject(object token, Func<object, JToken> serializeObj)
        {
            var dataArray = token as IEnumerable<object>;

            // single thing, just serialize it
            if (dataArray == null)
            {
                return serializeObj(token);
            }

            // serialize each element separately
            var data = new JArray();
            foreach (var obj in dataArray)
            {
                data.Add(serializeObj(obj));
            }

            return data;
        }

        private static JToken SerializeMinimalData(object properties, ApiResource resource)
        {
            var data = new JObject
            {
                ["type"] = resource.ResourceType.ToDashed(),
                ["id"] = JToken.FromObject(EnsureHasId(properties, resource))
            };

            return data;
        }

        private static JToken SerializeAttributes(object properties, ApiResource resource)
        {
            var attributes = new JObject();
            foreach (var attr in resource.Attributes)
            {
                var value = GetValue(attr.Name, properties);
                if (value != null)
                {
                    attributes.Add(attr.Name, JToken.FromObject(value));
                }
            }

            return attributes;
        }

        private static object GetValue(string name, object properties)
        {
            try
            {
                return properties.Send(name.ToPascalCase());
            }
            catch
            {
                return null;
            }

        }

        private static object GetId(object properties, ApiResource resource)
        {
            return GetValue(resource.IdProperty, properties);
        }

        private static object EnsureHasId(object properties, ApiResource resource)
        {
            var id = GetId(properties, resource);
            if (id == null)
            {
                throw new JsonApiException(ErrorType.Server, "Resources must have an id");
            }

            return id;
        }

        private JObject SerializeNull()
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
            {
                result["first"] = new Uri(left + queryStrings.FirstPage);
            }

            if (queryStrings.NextPage != null && count >= _paginationContext.PerPage)
            {
                result["next"] = new Uri(left + queryStrings.NextPage);
            }

            if (queryStrings.PreviousPage != null)
            {
                result["prev"] = new Uri(left + queryStrings.PreviousPage);
            }

            return result;
        }

        private JToken SerializeData(object properties)
        {
            var data = SerializeMinimalData(properties);

            data["attributes"] = SerializeAttributes(properties);
            data["relationships"] = SerializeRelationships(properties);

            if (_isCollection)
            {
                data["links"] = AddUrl(
                    new JObject(),
                    "self",
                    _urlBuilder.BuildCanonicalPath(_resource, (string)EnsureHasId(properties, _resource)));
            }

            return data;
        }

        private JToken SerializeMinimalData(object properties)
        {
            return SerializeMinimalData(properties, _resource);
        }

        private JToken SerializeAttributes(object properties)
        {
            return SerializeAttributes(properties, _resource);
        }

        private JToken SerializeRelationships(object properties)
        {
            var relationships = new JObject();

            foreach (var rel in _resource.Relationships)
            {
                relationships[rel.Name] = SerializeRelationship(rel, properties);
            }

            return relationships;
        }

        private JToken SerializeRelationship(ResourceRelationship relationship, object properties)
        {
            var relationshipValue = GetValue(relationship.Name, properties);

            // serialize the links part (so the data can be fetched)
            var objId = EnsureHasId(properties, _resource);
            var relToken = GetMinimumRelationship(
                objId.ToString(),
                relationship,
                relationshipValue != null ? GetId(relationshipValue, relationship.RelatedResource).ToString() : null);
            if (relationshipValue == null)
            {
                return relToken;
            }

            // only include data if it exists, otherwise just assume it should be fetched later
            var data = GetRelationshipData(relationship, relationshipValue);
            if (data != null)
            {
                relToken["data"] = data;
            }

            return relToken;
        }

        private JToken GetRelationshipData(ResourceRelationship relationship, object relationshipValues)
        {
            var data = SerializeArrayOrObject(
                relationshipValues,
                props =>
                {
                    var values = SerializeMinimalData(props, relationship.RelatedResource);
                    var includedData = values.DeepClone();
                    var url = _urlBuilder.BuildCanonicalPath(
                        relationship.RelatedResource,
                        (string)EnsureHasId(props, relationship.RelatedResource));

                    includedData["attributes"] = SerializeAttributes(props, relationship.RelatedResource);
                    includedData["links"] = AddUrl(new JObject(), "self", url);
                    if (!IsResourceIncluded(includedData))
                    {
                        _includedSection.Add(includedData);
                    }

                    return values;
                });
            return data;
        }

        private JToken GetMinimumRelationship(string id, ResourceRelationship relationship, string relationshipId)
        {
            var links = new JObject();
            AddUrl(links, "self", _urlBuilder.BuildRelationshipPath(_resource, id, relationship));
            AddUrl(links, "related", _urlBuilder.BuildRelationshipPath(_resource, id, relationship, relationshipId));

            return new JObject
            {
                ["links"] = links
            };
        }

        private bool IsResourceIncluded(JToken includedData)
        {
            return _includedSection.Any(t =>
                (string)t["type"] == (string)includedData["type"] &&
                (string)t["id"] == (string)includedData["id"]);
        }

        private JObject AddUrl(JObject @object, string name, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return @object;
            }

            var start = new Uri(_baseUrl.GetLeftPart(UriPartial.Authority).EnsureEndsWith("/"));
            @object.Add(name, new Uri(start, path.EnsureEndsWith("/")));

            return @object;
        }
    }
}