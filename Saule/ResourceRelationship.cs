namespace Saule
{
    /// <summary>
    /// Represents a related resource (to-one or to-many).
    /// </summary>
    public abstract class ResourceRelationship
    {
        internal ResourceRelationship(
            string name,
            RelationshipKind relationshipKind,
            string urlPath,
            ApiResource relationshipResource)
        {
            Name = name.ToCamelCase();
            RelationshipKind = relationshipKind;
            UrlPath = urlPath.ToDashed();
            RelatedResource = relationshipResource;
        }

        /// <summary>
        /// The name of this realtionship.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The kind of this relationship.
        /// </summary>
        public RelationshipKind RelationshipKind { get; }

        /// <summary>
        /// The definition of the related resource
        /// </summary>
        public ApiResource RelatedResource { get; }

        /// <summary>
        /// The pathspec of this relationship.
        /// </summary>
        public string UrlPath { get; }
    }

    /// <summary>
    /// Represents a related resource (to-one or to-many).
    /// </summary>
    public class ResourceRelationship<T> : ResourceRelationship where T : ApiResource, new()
    {
        internal ResourceRelationship(
            string name,
            RelationshipKind relationshipKind,
            string urlPath,
            ApiResource relationshipSource)
            : base(
              name,
               relationshipKind,
               urlPath,
               typeof(T).Equals(relationshipSource.GetType())
                   ? relationshipSource
                   : new T())
        {
        }
    }
}