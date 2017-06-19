using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ResourceGraphRelationship
    {
        public ResourceGraphRelationship(ResourceRelationship relationship, bool included, object obj)
        {
            relationship.ThrowIfNull(nameof(relationship));

            Included = included;
            SourceObject = obj;
            Relationship = relationship;
        }

        public bool Included { get; private set; }

        public object SourceObject { get; private set; }

        public ResourceRelationship Relationship { get; private set; }

        public override string ToString()
        {
            var type = SourceObject?.GetType().GetGenericTypeParameterOfCollection() ?? SourceObject?.GetType();

            return Relationship.Name + " <" + (type?.ToString() ?? "unknown") + ">";
        }
    }
}
