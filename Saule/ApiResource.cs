using System.Collections.Generic;

namespace Saule
{
    /// <summary>
    /// Represents a resource that can be consumed by clients.
    /// </summary>
    public abstract class ApiResource
    {
        private readonly List<ResourceAttribute> _attributes = new List<ResourceAttribute>();
        private readonly List<ResourceRelationship> _relationships = new List<ResourceRelationship>();

        /// <summary>
        /// The attributes of this resource.
        /// </summary>
        internal IEnumerable<ResourceAttribute> Attributes => _attributes;

        /// <summary>
        /// Resources related to this resource.
        /// </summary>
        internal IEnumerable<ResourceRelationship> Relationships => _relationships;

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
            OfType(name.ToUpperInvariant().EndsWith("RESOURCE") 
                ? name.Remove(name.Length - "RESOURCE".Length) 
                : name);
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
        protected ResourceAttribute Attribute(string name)
        {
            if (name.ToDashed() == "id") throw new JsonApiException("You cannot add an attribute named 'id'.");

            var result = new ResourceAttribute(name);

            _attributes.Add(result);

            return result;
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        protected ResourceRelationship BelongsTo<T>(string name) where T : ApiResource, new()
        {
            return BelongsTo<T>(name, name);
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        protected ResourceRelationship BelongsTo<T>(string name, string path) where T : ApiResource, new()
        {
            if (name.ToDashed() == "id") throw new JsonApiException("You cannot add a relationship named 'id'.");

            var result = new ResourceRelationship<T>(name, path, this);

            _relationships.Add(result);

            return result;
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        protected ResourceRelationship HasMany<T>(string name) where T : ApiResource, new()
        {
            return HasMany<T>(name, name);
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        protected ResourceRelationship HasMany<T>(string name, string path) where T : ApiResource, new()
        {
            if (name.ToDashed() == "id") throw new JsonApiException("You cannot add a relationship named 'id'.");

            var result = new ResourceRelationship<T>(name, path, this);

            _relationships.Add(result);

            return result;
        }
    }
}