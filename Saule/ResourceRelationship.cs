using System;

namespace Saule
{
    /// <summary>
    /// Represents a related resource (to-one or to-many).
    /// </summary>
    public class ResourceRelationship
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of this relationship.</param>
        /// <param name="relationshipKind">The kind of this relationship.</param>
        /// <param name="modelType">They type of the related resource.</param>
        /// <param name="urlPath">The pathspec of this relationship.</param>
        public ResourceRelationship(string name, RelationshipKind relationshipKind, Type modelType, string urlPath)
        {
            Name = name.ToCamelCase();
            RelationshipKind = relationshipKind;
            ModelType = modelType;
            UrlPath = urlPath.ToDashed();
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
        /// The type of the related resource.
        /// </summary>
        public Type ModelType { get; }

        /// <summary>
        /// The pathspec of this relationship.
        /// </summary>
        public string UrlPath { get; }
    }
}