namespace Saule
{
    /// <summary>
    /// Represents a related resource (to-one or to-many).
    /// </summary>
    public abstract class ResourceRelationship
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRelationship"/> class.
        /// </summary>
        /// <param name="name">The name of the reference on the resource that defines the relationship.</param>
        /// <param name="urlPath">
        /// The url path of this relationship relative to the resource url that holds
        /// the relationship.
        /// </param>
        /// <param name="kind">The kind of relationship.</param>
        /// <param name="relationshipResource">The specification of the related resource.</param>
        protected ResourceRelationship(
            string name,
            string urlPath,
            RelationshipKind kind,
            ApiResource relationshipResource)
        {
            Name = name.ToDashed();
            UrlPath = urlPath.ToDashed();
            RelatedResource = relationshipResource;
            Kind = kind;
        }

        /// <summary>
        /// The name of this relationship.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The definition of the related resource
        /// </summary>
        public ApiResource RelatedResource { get; }

        /// <summary>
        /// The pathspec of this relationship.
        /// </summary>
        public string UrlPath { get; }

        /// <summary>
        /// The kind of relationship.
        /// </summary>
        public RelationshipKind Kind { get; }
    }
}