using System;

namespace Saule
{
    /// <summary>
    /// Represents a related resource (to-one or to-many).
    /// </summary>
    public class ResourceRelationship
    {
        internal ResourceRelationship(
            string name,
            RelationshipKind relationshipKind,
            Type resourceType,
            string urlPath,
            ApiResource resource)
        {
            Name = name.ToCamelCase();
            RelationshipKind = relationshipKind;
            UrlPath = urlPath.ToDashed();

            RelatedResource = resourceType.Equals(resource.GetType())
                ? RelatedResource = resource
                : RelatedResource = (ApiResource)Activator.CreateInstance(resourceType);
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
}