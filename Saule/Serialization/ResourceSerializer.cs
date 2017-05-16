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
        private readonly IUrlPathBuilder _urlBuilder;
        private readonly ResourceGraphPathSet _includedGraphPaths;
        private JsonSerializer _serializer;

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
            _includedGraphPaths = IncludedGraphPathsFromContext(includingContext);
        }

        public JObject Serialize()
        {
            return Serialize(new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public JObject Serialize(JsonSerializer serializer)
        {
            serializer.ContractResolver = new JsonApiContractResolver();
            _serializer = serializer;

            if (_value == null)
            {
                return SerializeNull();
            }

            var graph = new ResourceGraph(_value, _resource, _includedGraphPaths);
            var dataSection = SerializeData(graph);
            var includesSection = SerializeIncludes(graph);

            var result = new JObject
            {
                ["data"] = dataSection,
                ["links"] = new JObject
                {
                    ["self"] = new JValue(_baseUrl)
                }

                ["links"] = CreateTopLevelLinks(dataSection is JArray ? dataSection.Count() : 0)
            };

            if (includesSection != null && includesSection.Count > 0)
            {
                result["included"] = includesSection;
            }

            return result;
        }

        private ResourceGraphPathSet IncludedGraphPathsFromContext(IncludingContext context)
        {
            if (context == null)
            {
                return new ResourceGraphPathSet.All();
            }
            else if (context.Includes != null && context.Includes.Count() > 0)
            {
                return new ResourceGraphPathSet(_includingContext.Includes.Select(i => i.Name));
            }
            else if (context.DisableDefaultIncluded)
            {
                return new ResourceGraphPathSet.None();
            }
            else
            {
                return new ResourceGraphPathSet.All();
            }
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

        private JObject SerializeNull()
        {
            return new JObject
            {
                ["data"] = null,
                ["links"] = CreateTopLevelLinks(0)
            };
        }

        private JToken SerializeData(ResourceGraph graph)
        {
            var isCollection = _value is System.Collections.IEnumerable;

            var tokens = graph.DataNodes.Select(n => SerializeNode(n, isCollection));

            if (isCollection)
            {
                if (tokens.Count() == 0)
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
                if (tokens.Count() == 0)
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
            var tokens = graph.IncludedNodes.Select(n => SerializeNode(n, true));
            if (tokens.Count() == 0)
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
                response["links"] = AddUrl(
                    new JObject(),
                    "self",
                    _urlBuilder.BuildCanonicalPath(node.Resource, node.Key.Id.ToString()));
            }

            var attributes = SerializeAttributes(node);
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
                    node.SourceObject.IncludesProperty(a.PropertyName))
                .Select(a =>
                    new
                    {
                        Key = a.Name,
                        Value = node.SourceObject.GetValueOfProperty(a.PropertyName)
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
                var item = new JObject();

                var data = SerializeRelationshipData(kv.Value);

                var relationshipId = default(string);

                if (data != null
                    && kv.Value.Relationship.Kind == RelationshipKind.BelongsTo
                    && kv.Value.SourceObject != null)
                {
                    relationshipId = (string)data["id"];
                }

                var links = new JObject();
                AddUrl(links, "self", _urlBuilder.BuildRelationshipPath(node.Resource, node.Key.Id.ToString(), kv.Value.Relationship));
                AddUrl(links, "related", _urlBuilder.BuildRelationshipPath(node.Resource, node.Key.Id.ToString(), kv.Value.Relationship, relationshipId));

                item["links"] = links;

                if (data != null)
                {
                    item["data"] = data;
                }

                response[kv.Key] = item;
            }

            return response;
        }

        private JToken SerializeRelationshipData(ResourceGraphRelationship relationship)
        {
            if (!relationship.Included)
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