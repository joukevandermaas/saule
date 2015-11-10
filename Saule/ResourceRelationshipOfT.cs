using System.Diagnostics.CodeAnalysis;

namespace Saule
{
    /// <summary>
    /// Represents a related resource (to-one or to-many).
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name must match first type name",
        Justification = "Non-generic version exists")]
    internal class ResourceRelationship<T> : ResourceRelationship
            where T : ApiResource, new()
        {
        internal ResourceRelationship(string name, string urlPath, RelationshipKind kind, T resource)
            : base(name, urlPath, kind, resource)
        {
        }
    }
}