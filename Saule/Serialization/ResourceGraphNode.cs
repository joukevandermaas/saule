using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ResourceGraphNode
    {
        public ResourceGraphNode(
            object obj,
            ApiResource resource,
            ResourceGraphPathSet includePaths,
            int graphDepth,
            string propertyName = null)
        {
            obj.ThrowIfNull(nameof(obj));
            resource.ThrowIfNull(nameof(resource));
            includePaths.ThrowIfNull(nameof(includePaths));

            Key = new ResourceGraphNodeKey(obj, resource);
            SourceObject = obj;
            IncludePaths = includePaths;
            PropertyName = propertyName;
            Resource = resource;
            GraphDepth = graphDepth;

            Relationships = Resource.Relationships
                    .Select(r => new
                    {
                        key = r.Name,
                        value = new ResourceGraphRelationship(r, includePaths.MatchesProperty(r.PropertyName), SourceObject.GetValueOfProperty(r.PropertyName))
                    }).ToDictionary(a => a.key, a => a.value);
        }

        public ResourceGraphNodeKey Key { get; private set; }

        public ResourceGraphPathSet IncludePaths { get; private set; }

        public object SourceObject { get; private set; }

        public ApiResource Resource { get; private set; }

        public int GraphDepth { get; set; }

        public string PropertyName { get; private set; }

        public IReadOnlyDictionary<string, ResourceGraphRelationship> Relationships { get; private set; }

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}
