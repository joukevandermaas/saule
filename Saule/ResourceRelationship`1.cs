namespace Saule
{

    /// <summary>
    /// Represents a related resource (to-one or to-many).
    /// </summary>
    internal class ResourceRelationship<T> : ResourceRelationship where T : ApiResource, new()
    {
        internal ResourceRelationship(string name, string urlPath, RelationshipKind kind, T resource)
            : base(name, urlPath, kind, resource)
        {
        }
    }
}