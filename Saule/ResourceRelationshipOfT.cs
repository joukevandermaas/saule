using System.Diagnostics.CodeAnalysis;

namespace Saule
{
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name must match first type name",
        Justification = "Non-generic version exists")]
    internal class ResourceRelationship<T> : ResourceRelationship
            where T : ApiResource, new()
        {
        internal ResourceRelationship(string name, string urlPath, RelationshipKind kind, T resource, LinkType withLinks)
            : base(name, urlPath, kind, resource, withLinks)
        {
        }
    }
}