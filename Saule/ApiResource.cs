using System.Collections.Generic;

namespace Saule
{
    /// <summary>
    /// Represents a resource that can be consumed by clients.
    /// </summary>
    public abstract class ApiResource
    {
        private List<ResourceAttribute> _attributes = new List<ResourceAttribute>();
        private List<ResourceRelationship> _relationships = new List<ResourceRelationship>();

        /// <summary>
        /// The attributes of this resource.
        /// </summary>
        public IEnumerable<ResourceAttribute> Attributes => _attributes;

        /// <summary>
        /// Resources related to this resource.
        /// </summary>
        public IEnumerable<ResourceRelationship> Relationships => _relationships;

        /// <summary>
        /// The type name of this resource.
        /// </summary>
        public string ResourceType { get; private set; }

        /// <summary>
        ///
        /// </summary>
        protected ApiResource()
        {
            var name = GetType().Name;
            if (name.ToUpperInvariant().EndsWith("RESOURCE"))
            {
                OfType(name.Remove(name.Length - "RESOURCE".Length));
            }
            else
            {
                OfType(name);
            }
        }

        /// <summary>
        /// Customize the type name of this resource. The default value
        /// is the name of the class (without 'Resource', if it exists).
        /// </summary>
        /// <param name="value">The type of the resource.</param>
        protected void OfType(string value)
        {
            ResourceType = value.ToDashed();
        }

        /// <summary>
        /// Specify an attribute of this resource.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        protected void Attribute(string name)
        {
            if (name.ToDashed() == "id") throw new JsonApiException("You cannot add an attribute named 'id'.");

            _attributes.Add(new ResourceAttribute(name));
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        protected void BelongsTo<T>(string name) where T : ApiResource, new()
        {
            BelongsTo<T>(name, name);
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        protected void BelongsTo<T>(string name, string path) where T : ApiResource, new()
        {
            if (name.ToDashed() == "id") throw new JsonApiException("You cannot add a relationship named 'id'.");

            _relationships.Add(new ResourceRelationship<T>(name, RelationshipKind.Single, path, this));
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        protected void HasMany<T>(string name) where T : ApiResource, new()
        {
            HasMany<T>(name, name);
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        protected void HasMany<T>(string name, string path) where T : ApiResource, new()
        {
            if (name.ToDashed() == "id") throw new JsonApiException("You cannot add a relationship named 'id'.");

            _relationships.Add(new ResourceRelationship<T>(name, RelationshipKind.Many, path, this));
        }
    }
}