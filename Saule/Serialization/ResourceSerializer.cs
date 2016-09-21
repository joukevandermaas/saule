using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Queries.Including;
using Saule.Queries.Pagination;

namespace Saule.Serialization
{
    internal class ResourceSerializer
    {
        private readonly Uri _baseUrl;
        private readonly PaginationContext _paginationContext;
        private readonly IncludingContext _includingContext;
        private readonly ApiResource _resource;
        private readonly object _value;
        private readonly JArray _includedSection;
        private readonly IUrlPathBuilder _urlBuilder;
        private bool _isCollection;

        public ResourceSerializer(
            object value,
            ApiResource type,
            Uri baseUrl,
            IUrlPathBuilder urlBuilder,
            PaginationContext paginationContext,
            IncludingContext includingContext)
        {
            _urlBuilder = urlBuilder;
            _resource = type;
            _value = value;
            _baseUrl = baseUrl;
            _paginationContext = paginationContext;
            _includingContext = includingContext;
            _includedSection = new JArray();
        }

        public JObject Serialize()
        {
            return Serialize(new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public JObject Serialize(JsonSerializer serializer)
        {
            serializer.ContractResolver = new JsonApiContractResolver();
            if (_value == null)
            {
                return SerializeNull();
            }

            var objectJson = JToken.FromObject(_value, serializer);
            _isCollection = objectJson is JArray;

            var result = new JObject
            {
                ["data"] = SerializeArrayOrObject(_resource, objectJson, SerializeData),
                ["links"] = new JObject
                {
                    ["self"] = new JValue(_baseUrl)
                }

                ["links"] = CreateTopLevelLinks(_isCollection ? objectJson.Count() : 0)
            };

            if (_includedSection.Count > 0)
            {
                result["included"] = _includedSection;
            }

            return result;
        }

        private static JToken SerializeArrayOrObject(ApiResource resource, JToken token, Func<ApiResource, IDictionary<string, JToken>, JToken> serializeObj)
        {
            var dataArray = token as JArray;

            // single thing, just serialize it
            if (dataArray == null)
            {
                return token is JObject ? serializeObj(resource, (JObject)token) : null;
            }

            // serialize each element separately
            var data = new JArray();
            foreach (var obj in dataArray.OfType<JObject>())
            {
                data.Add(serializeObj(resource, obj));
            }

            return data;
        }

        private static JToken SerializeMinimalData(IDictionary<string, JToken> properties, ApiResource resource)
        {
            var data = new JObject
            {
                ["type"] = resource.ResourceType.ToDashed(),
                ["id"] = EnsureHasId(properties, resource)
            };

            return data;
        }

        private static JToken SerializeAttributes(IDictionary<string, JToken> properties, ApiResource resource)
        {
            var attributes = new JObject();
            foreach (var attr in resource.Attributes)
            {
                var value = GetValue(attr.Name, properties);
                if (value != null)
                {
                    attributes.Add(attr.Name, value);
                }
            }

            return attributes;
        }

        private static JToken GetValue(string name, IDictionary<string, JToken> properties)
        {
            return properties[name.ToDashed()];
        }

        private static JToken GetId(IDictionary<string, JToken> properties, ApiResource resource)
        {
            return GetValue(resource.IdProperty, properties);
        }

        private static JToken EnsureHasId(IDictionary<string, JToken> properties, ApiResource resource)
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

        private JToken SerializeData(ApiResource resource, IDictionary<string, JToken> properties)
        {
            var data = SerializeMinimalData(properties);

            data["attributes"] = SerializeAttributes(properties);
            data["relationships"] = SerializeRelationships(resource, properties);

            if (_isCollection)
            {
                data["links"] = AddUrl(
                    new JObject(),
                    "self",
                    _urlBuilder.BuildCanonicalPath(_resource, (string)EnsureHasId(properties, _resource)));
            }

            return data;
        }

        private JToken SerializeMinimalData(IDictionary<string, JToken> properties)
        {
            return SerializeMinimalData(properties, _resource);
        }

        private JToken SerializeAttributes(IDictionary<string, JToken> properties)
        {
            return SerializeAttributes(properties, _resource);
        }

        private JToken SerializeRelationships(ApiResource resource, IDictionary<string, JToken> properties)
        {
            var relationships = new JObject();

            foreach (var rel in resource.Relationships)
            {
                relationships[rel.Name] = SerializeRelationship(resource, rel, properties);
            }

            return relationships;
        }

        private JToken SerializeRelationship(ApiResource resource, ResourceRelationship relationship, IDictionary<string, JToken> properties)
        {
            var relationshipValues = GetValue(relationship.Name, properties);
            var relationshipProperties = relationshipValues as JObject;

            // serialize the links part (so the data can be fetched)
            var objId = EnsureHasId(properties, resource);
            var relToken = GetMinimumRelationship(
                objId.ToString(),
                resource,
                relationship,
                relationshipProperties != null ? (string)GetId(relationshipProperties, relationship.RelatedResource) : null);
            if (relationshipValues == null)
            {
                return relToken;
            }

            // only include data if it exists, otherwise just assume it should be fetched later
            var data = GetRelationshipData(relationship, relationshipValues);
            if (data != null)
            {
                relToken["data"] = data;
            }

            return relToken;
        }

        private JToken GetRelationshipData(ResourceRelationship relationship, JToken relationshipValues)
        {
            var data = SerializeArrayOrObject(
                relationship.RelatedResource,
                relationshipValues,
                (resource, props) =>
                {
                    var values = SerializeMinimalData(props, relationship.RelatedResource);
                    var includedData = values.DeepClone();
                    var url = _urlBuilder.BuildCanonicalPath(
                        relationship.RelatedResource,
                        (string)EnsureHasId(props, relationship.RelatedResource));

                    includedData["attributes"] = SerializeAttributes(props, relationship.RelatedResource);
                    includedData["relationships"] = SerializeRelationships(resource, props);
                    includedData["links"] = AddUrl(new JObject(), "self", url);

                    var includes = _includingContext?.Includes?.Select(x => x.Name).ToList();
                    if (includes != null)
                    {
                        var newIncludes = new List<string>();
                        foreach (var include in includes)
                        {
                            var temp = include.Split('.');
                            newIncludes.AddRange(temp);
                        }
                        includes.AddRange(newIncludes.Except(includes));
                    }

                    if (includes != null && includes?.Count() != 0)
                    {
                        if (includes.Contains(relationship.Name.ToPascalCase()) && !IsResourceIncluded(includedData))
                        {
                            _includedSection.Add(includedData);
                        }
                    }
                    else if (!IsResourceIncluded(includedData))
                    {
                        _includedSection.Add(includedData);
                    }

                    return values;
                });
            return data;
        }

        private JToken GetMinimumRelationship(string id, ApiResource resource, ResourceRelationship relationship, string relationshipId)
        {
            var links = new JObject();
            AddUrl(links, "self", _urlBuilder.BuildRelationshipPath(resource, id, relationship));
            AddUrl(links, "related", _urlBuilder.BuildRelationshipPath(resource, id, relationship, relationshipId));

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