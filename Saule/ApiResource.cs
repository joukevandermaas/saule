using System;
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
                WithType(name.Remove(name.Length - "RESOURCE".Length));
            }
            else
            {
                WithType(name);
            }
        }

        /// <summary>
        /// Customize the type name of this resource. The default value
        /// is the name of the class (without 'Resource', if it exists).
        /// </summary>
        /// <param name="value">The type of the resource.</param>
        protected void WithType(string value)
        {
            ResourceType = value.ToDashed();
        }

        /// <summary>
        /// Specify an attribute of this resource.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        protected void Attribute(string name)
        {
            _attributes.Add(new ResourceAttribute(name));
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="type">The type of the related type.</param>
        protected void BelongsTo(string name, Type type)
        {
            BelongsTo(name, type, name);
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="type">The type of the related type.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        protected void BelongsTo(string name, Type type, string path)
        {
            _relationships.Add(new ResourceRelationship(name, RelationshipKind.Single, type, path, this));
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="type">The type of the related type.</param>
        protected void HasMany(string name, Type type)
        {
            HasMany(name, type, name);
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="type">The type of the related type.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        protected void HasMany(string name, Type type, string path)
        {
            _relationships.Add(new ResourceRelationship(name, RelationshipKind.Many, type, path, this));
        }
    }
}