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
        /// <param name="withLinks">The defined <see cref="LinkType"/> to be generated for this relationship.</param>
        protected ResourceRelationship(
            string name,
            string urlPath,
            RelationshipKind kind,
            ApiResource relationshipResource,
            LinkType withLinks)
        {
            Name = name.ToDashed();
            PropertyName = name.ToPascalCase();
            UrlPath = urlPath.ToDashed();
            RelatedResource = relationshipResource;
            Kind = kind;
            LinkType = withLinks;
        }

        /// <summary>
        /// Gets the name of the relationship in dashed JSON API format.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the relationship in PascalCase.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the definition of the related resource
        /// </summary>
        public ApiResource RelatedResource { get; }

        /// <summary>
        /// Gets the pathspec of this relationship.
        /// </summary>
        public string UrlPath { get; }

        /// <summary>
        /// Gets the kind of relationship.
        /// </summary>
        public RelationshipKind Kind { get; }

        /// <summary>
        /// Gets the defined <see cref="LinkType"/> to be generated for this relationship.
        /// </summary>
        public LinkType LinkType { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return PropertyName + " " + Kind + "<" + RelatedResource + ">";
        }
    }
}