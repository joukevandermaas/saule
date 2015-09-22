using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Humanizer;

namespace Saule
{
    /// <summary>
    /// Represents a resource that can be consumed by clients.
    /// </summary>
    public abstract class ApiResource
    {
        private static readonly ConcurrentDictionary<Type, ApiResource> Resources = new ConcurrentDictionary<Type, ApiResource>();
        private readonly List<ResourceAttribute> _attributes = new List<ResourceAttribute>();
        private readonly List<ResourceRelationship> _relationships = new List<ResourceRelationship>();

        internal IEnumerable<ResourceAttribute> Attributes => _attributes;

        internal IEnumerable<ResourceRelationship> Relationships => _relationships;

        /// <summary>
        /// The url path of this resource.
        /// </summary>
        public string UrlPath { get; private set; }

        /// <summary>
        /// The type name of this resource.
        /// </summary>
        public string ResourceType { get; private set; }

        /// <summary>
        ///
        /// </summary>
        protected ApiResource()
        {
            var type = GetType();

            var name = type.Name;
            OfType(name.ToUpperInvariant().EndsWith("RESOURCE") 
                ? name.Remove(name.Length - "RESOURCE".Length) 
                : name);

            Resources.TryAdd(type, this);
        }

        /// <summary>
        /// Customize the type name of this resource. The default value
        /// is the name of the class (without 'Resource', if it exists).
        /// </summary>
        /// <param name="value">The type of the resource.</param>
        protected void OfType(string value)
        {
            OfType(value, value.Pluralize(inputIsKnownToBeSingular: false));
        }

        /// <summary>
        /// Customize the type name of this resource. The default value
        /// is the name of the class (without 'Resource', if it exists).
        /// </summary>
        /// <param name="value">The type of the resource.</param>
        /// <param name="path">The url pathspec of this relationship (default is the 
        /// pluralized version of the type name)</param>
        protected void OfType(string value, string path)
        {
            ResourceType = value.ToDashed();
            UrlPath = path.ToDashed().EnsureStartsWith("/");
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

            var resource = GetUniqueResource<T>();
            var result = new ResourceRelationship<T>(name, path, RelationshipKind.BelongsTo, resource);

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

            var resource = GetUniqueResource<T>();
            var result = new ResourceRelationship<T>(name, path, RelationshipKind.HasMany, resource);

            _relationships.Add(result);

            return result;
        }

        private static T GetUniqueResource<T>() where T : ApiResource, new()
        {
            var type = typeof (T);
            var resource = Resources.ContainsKey(type)
                ? Resources[type] as T
                : new T();
            return resource;
        }
    }
}