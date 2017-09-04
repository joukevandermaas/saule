using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Saule.Queries.Including;

namespace Saule.Serialization
{
    internal class ResourceGraph
    {
        private readonly IDictionary<ResourceGraphNodeKey, ResourceGraphNode> _nodes;

        public ResourceGraph(
            object obj,
            ApiResource resource,
            ResourceGraphPathSet includePaths)
        {
            obj.ThrowIfNull(nameof(obj));
            resource.ThrowIfNull(nameof(resource));
            includePaths.ThrowIfNull(nameof(includePaths));

            _nodes = new Dictionary<ResourceGraphNodeKey, ResourceGraphNode>();

            Build(obj, resource, includePaths, 0);
        }

        public IEnumerable<ResourceGraphNode> DataNodes
        {
            get
            {
                return _nodes.Values.Where(v => v.GraphDepth == 0);
            }
        }

        public IEnumerable<ResourceGraphNode> IncludedNodes
        {
            get
            {
                return _nodes.Values.Where(v => v.GraphDepth > 0);
            }
        }

        private void Build(
            object obj,
            ApiResource resource,
            ResourceGraphPathSet includePaths,
            int depth,
            string propertyName = null)
        {
            if (obj == null)
            {
                // end of the line - obj will be a leaf node in our graph
                return;
            }

            if (obj.IsCollectionType())
            {
                foreach (var o in (IEnumerable)obj)
                {
                    Build(o, resource, includePaths, depth);
                }

                return;
            }

            // keys (type & id pair) uniquely identifier each resource in a compount document
            var key = new ResourceGraphNodeKey(obj, resource);
            var existingNode = _nodes.ContainsKey(key) ? _nodes[key] : null;

            if (existingNode == null)
            {
                // this is the first time we have seen this node
                existingNode = new ResourceGraphNode(obj, resource, includePaths, depth, propertyName);
            }
            else if (!existingNode.IncludePaths.Equals(includePaths))
            {
                /*
                 * We've seen this node before but this time it's include paths are different
                 * to those we are currently applying.This is usually because a resource
                 * is present at two or more distinct locations within an object graph.
                 *
                 * We must combine our current include paths with those of the already built node
                 * and rebuild to capture any new relationships to include.
                 */

                includePaths = existingNode.IncludePaths.Union(includePaths);
                existingNode = new ResourceGraphNode(obj, resource, includePaths, depth, propertyName);
            }
            else if (existingNode.GraphDepth > depth)
            {
                /*
                 * We've seen this node before but this time we're closer to the root node.
                 * We must update it's depth to the new lower value so we can later on identify
                 * nodes that should be serialized into the data section.
                 */

                existingNode.GraphDepth = depth;
                return;
            }
            else
            {
                return;
            }

            _nodes[key] = existingNode;

            foreach (var r in resource.Relationships
                    .Where(r => includePaths.MatchesProperty(r.PropertyName)))
            {
                // Build a new set of include paths with the relationship property as the root
                var childIncludes = includePaths.PathSetForChildProperty(r.PropertyName);

                if (r.Kind == RelationshipKind.HasMany)
                {
                    var collection = (IEnumerable)obj.GetValueOfProperty(r.PropertyName);

                    if (collection != null)
                    {
                        foreach (var o in collection)
                        {
                            // Add the relationship member to the graph
                            Build(o, r.RelatedResource, childIncludes, depth + 1, r.PropertyName);
                        }
                    }
                }
                else
                {
                    var o = obj.GetValueOfProperty(r.PropertyName);
                    if (o != null)
                    {
                        // Add the relationship member to the graph
                        Build(o, r.RelatedResource, childIncludes, depth + 1, r.PropertyName);
                    }
                }
            }
        }
    }
}
