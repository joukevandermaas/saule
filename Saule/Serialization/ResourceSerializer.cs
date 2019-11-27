using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Queries.Fieldset;
using Saule.Queries.Including;
using Saule.Queries.Pagination;

namespace Saule.Serialization
{
    internal class ResourceSerializer
    {
        private readonly Uri _baseUrl;
        private readonly PaginationContext _paginationContext;
        private readonly IncludeContext _includeContext;
        private readonly FieldsetContext _fieldsetContext;
        private readonly ApiResource _resource;
        private readonly object _value;
        private readonly IPropertyNameConverter _propertyNameConverter;
        private readonly IUrlPathBuilder _urlBuilder;
        private readonly ResourceGraphPathSet _includedGraphPaths;
        private JsonSerializer _serializer;

        public ResourceSerializer(
            object value,
            ApiResource type,
            Uri baseUrl,
            IUrlPathBuilder urlBuilder,
            PaginationContext paginationContext,
            IncludeContext includeContext,
            FieldsetContext fieldsetContext,
            IPropertyNameConverter propertyNameConverter = null)
        {
            _propertyNameConverter = propertyNameConverter ?? new DefaultPropertyNameConverter();
            _urlBuilder = urlBuilder;
            _resource = type;
            _value = value;
            _baseUrl = baseUrl;
            _paginationContext = paginationContext;
            _includeContext = includeContext;
            _fieldsetContext = fieldsetContext;
            _includedGraphPaths = IncludedGraphPathsFromContext(includeContext);
        }

        public JObject Serialize()
        {
            return Serialize(new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public JObject Serialize(JsonSerializer serializer)
        {
            serializer.ContractResolver = new JsonApiContractResolver(_propertyNameConverter);
            _serializer = serializer;

            if (_value == null)
            {
                return SerializeNull();
            }

            var graph = new ResourceGraph(_value, _resource, _includedGraphPaths);
            var dataSection = SerializeData(graph);
            var includesSection = SerializeIncludes(graph);
            var metaSection = SerializeMetadata();

            var result = new JObject
            {
                ["data"] = dataSection
            };

            var isCollection = _value.IsCollectionType();
            string id = null;
            if (!isCollection)
            {
                id = dataSection["id"]?.ToString();
            }

            var links = CreateTopLevelLinks(dataSection is JArray ? dataSection.Count() : 0, id);

            if (links.HasValues)
            {
                result.Add("links", links);
            }

            if (includesSection != null && includesSection.Count > 0)
            {
                result["included"] = includesSection;
            }

            if (metaSection != null)
            {
                result["meta"] = metaSection;
            }

            return result;
        }

        private JToken SerializeMetadata()
        {
            var valueType = _value.GetType();
            var isCollection = false;

            if (typeof(IEnumerable).IsAssignableFrom(valueType))
            {
                isCollection = true;
                valueType = valueType.GetGenericTypeParameterOfCollection() ?? valueType;
            }

            var metaObject = _resource.GetMetadata(_value, valueType, isCollection);

            if (metaObject is JToken)
            {
                return metaObject as JToken;
            }

            return metaObject == null ? null : JToken.FromObject(metaObject, _serializer);
        }

        private ResourceGraphPathSet IncludedGraphPathsFromContext(IncludeContext context)
        {
            if (context == null)
            {
                return new ResourceGraphPathSet.All();
            }
            else if (context.Includes != null && context.Includes.Any())
            {
                return new ResourceGraphPathSet(_includeContext.Includes.Select(i => i.Name));
            }
            else
            {
                return new ResourceGraphPathSet.All();
            }
        }

        private JToken CreateTopLevelLinks(int count, string id = null)
        {
            var result = new JObject();

            // to preserve back compatibility if Self is enabled, then we also render it. Or if TopSelf is enabled
            if (_resource.LinkType.HasFlag(LinkType.TopSelf) || _resource.LinkType.HasFlag(LinkType.Self))
            {
                if (id != null && !_baseUrl.AbsolutePath.EndsWith(id, StringComparison.InvariantCultureIgnoreCase))
                {
                    AddUrl(result, "self", _urlBuilder.BuildCanonicalPath(_resource, id));
                }
                else
                {
                    result.Add("self", _baseUrl.ToString());
                }
            }

            var queryStrings = new PaginationQuery(_paginationContext, _value);

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

            if (queryStrings.LastPage != null)
            {
                result["last"] = new Uri(left + queryStrings.LastPage);
            }

            return result;
        }

        private JObject SerializeNull()
        {
            var result = new JObject
            {
                ["data"] = null
            };

            var links = CreateTopLevelLinks(0);

            if (links.HasValues)
            {
                result.Add("links", links);
            }

            return result;
        }

        private JToken SerializeData(ResourceGraph graph)
        {
            var isCollection = _value.IsCollectionType();

            var tokens = graph.DataNodes.Select(n => SerializeNode(n, isCollection));

            if (isCollection)
            {
                if (!tokens.Any())
                {
                    return new JArray();
                }
                else
                {
                    return JArray.FromObject(tokens);
                }
            }
            else
            {
                if (!tokens.Any())
                {
                    return JValue.CreateNull();
                }
                else
                {
                    return tokens.First();
                }
            }
        }

        private JArray SerializeIncludes(ResourceGraph graph)
        {
            var nodes = graph.IncludedNodes;

            // if we have an including context and DisableDefaultIncluded is set
            if (_includeContext != null && _includeContext.DisableDefaultIncluded)
            {
                // if we have specific includes filter nodes otherwise bail with null
                if (_includeContext.Includes != null)
                {
                    nodes = nodes.Where(n => _includeContext.Includes.Any(p => p.Name == n.PropertyName));
                }
                else
                {
                    return null;
                }
            }

            var tokens = nodes.Select(n => SerializeNode(n, true));

            if (!tokens.Any())
            {
                return null;
            }
            else
            {
                return JArray.FromObject(tokens);
            }
        }

        private JObject SerializeNode(ResourceGraphNode node, bool isCollection)
        {
            var response = new JObject
            {
                ["type"] = node.Key.Type,
                ["id"] = JToken.FromObject(node.Key.Id)
            };

            if (isCollection)
            {
                var self = _urlBuilder.BuildCanonicalPath(node.Resource, node.Key.Id.ToString());

                if (!string.IsNullOrEmpty(self) && node.Resource.LinkType.HasFlag(LinkType.Self))
                {
                    response["links"] = AddUrl(new JObject(), "self", self);
                }
            }

            JObject attributes = null;
            if (_fieldsetContext != null && _fieldsetContext.Properties.Count(property => property.Type == node.Key.Type) > 0)
            {
                FieldsetProperty fieldset = _fieldsetContext.Properties.Where(property => property.Type == node.Key.Type).First();
                attributes = SerializeAttributes(node, fieldset);
            }
            else
            {
                attributes = SerializeAttributes(node);
            }

            if (attributes != null)
            {
                response["attributes"] = attributes;
            }

            var relationships = SerializeRelationships(node);
            if (relationships != null)
            {
                response["relationships"] = relationships;
            }

            return response;
        }

        private JObject SerializeAttributes(ResourceGraphNode node)
        {
            var attributeHash = node.Resource.Attributes
                .Where(a =>
                    node.SourceObject.IncludesProperty(_propertyNameConverter.ToModelPropertyName(a.InternalName)))
                .Select(a =>
                    new
                    {
                        Key = _propertyNameConverter.ToJsonPropertyName(a.InternalName),
                        Value = node.SourceObject.GetValueOfProperty(_propertyNameConverter.ToModelPropertyName(a.InternalName))
                    })
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value);

            return JObject.FromObject(attributeHash, _serializer);
        }

        private JObject SerializeAttributes(ResourceGraphNode node, FieldsetProperty fieldset)
        {
            var attributeHash = node.Resource.Attributes
                .Where(a =>
                    node.SourceObject.IncludesProperty(_propertyNameConverter.ToModelPropertyName(a.InternalName)) && fieldset.Fields.Contains(a.InternalName.ToComparablePropertyName()))
                .Select(a =>
                    new
                    {
                        Key = _propertyNameConverter.ToJsonPropertyName(a.InternalName),
                        Value = node.SourceObject.GetValueOfProperty(_propertyNameConverter.ToModelPropertyName(a.InternalName))
                    })
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value);

            return JObject.FromObject(attributeHash, _serializer);
        }

        private JObject SerializeRelationships(ResourceGraphNode node)
        {
            if (node.Relationships.Count == 0)
            {
                return null;
            }

            var response = new JObject();

            foreach (var kv in node.Relationships)
            {
                var relationship = kv.Value.Relationship;

                var item = new JObject();

                var data = SerializeRelationshipData(node, kv.Value);

                var relationshipId = default(string);

                if (data != null
                    && relationship.Kind == RelationshipKind.BelongsTo
                    && kv.Value.SourceObject != null)
                {
                    relationshipId = (string)data["id"];
                }

                var links = new JObject();
                var self = _urlBuilder.BuildRelationshipPath(node.Resource, node.Key.Id.ToString(), relationship);
                var related = _urlBuilder.BuildRelationshipPath(node.Resource, node.Key.Id.ToString(), relationship, relationshipId);

                if (!string.IsNullOrEmpty(self) && relationship.LinkType.HasFlag(LinkType.Self))
                {
                    AddUrl(links, "self", self);
                }

                if (!string.IsNullOrEmpty(related) && relationship.LinkType.HasFlag(LinkType.Related))
                {
                    AddUrl(links, "related", related);
                }

                if (links.HasValues)
                {
                    item["links"] = links;
                }

                if (data != null && kv.Value != null)
                {
                    item["data"] = data;
                }

                response[_propertyNameConverter.ToJsonPropertyName(kv.Key)] = item;
            }

            return response;
        }

        private JToken SerializeRelationshipData(ResourceGraphNode node, ResourceGraphRelationship relationship)
        {
            // short circuit if not included in graph
            if (!relationship.Included)
            {
                return null;
            }

            // check if the relationship property exists on the underlying model and if not bail with null
            // NOTE: this logic refers to https://github.com/joukevandermaas/saule/issues/159
            if (node.SourceObject.GetType().GetProperty(relationship.Relationship.PropertyName) == null)
            {
                return null;
            }
            else if (relationship.Relationship.Kind == RelationshipKind.BelongsTo)
            {
                if (relationship.SourceObject == null)
                {
                    return JValue.CreateNull();
                }
                else
                {
                    return JObject.FromObject(new ResourceGraphNodeKey(relationship.SourceObject, relationship.Relationship.RelatedResource));
                }
            }
            else if (relationship.Relationship.Kind == RelationshipKind.HasMany)
            {
                var content = new JArray();
                foreach (var o in (System.Collections.IEnumerable)relationship.SourceObject ?? new object[0])
                {
                    content.Add(JObject.FromObject(new ResourceGraphNodeKey(o, relationship.Relationship.RelatedResource)));
                }

                return content;
            }
            else
            {
                return null;
            }
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