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
        private static readonly ConcurrentDictionary<Type, ApiResource> Resources =
            new ConcurrentDictionary<Type, ApiResource>();

        private readonly List<ResourceAttribute> _attributes = new List<ResourceAttribute>();
        private readonly List<ResourceRelationship> _relationships = new List<ResourceRelationship>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResource"/> class.
        /// </summary>
        protected ApiResource()
        {
            var type = GetType();

            var name = type.Name;
            OfType(name.ToUpperInvariant().EndsWith("RESOURCE")
                ? name.Remove(name.Length - "RESOURCE".Length)
                : name);

            WithId("Id");

            Resources.TryAdd(type, this);
        }

        /// <summary>
        /// Gets the url path of this resource.
        /// </summary>
        public string UrlPath { get; private set; }

        /// <summary>
        /// Gets the type name of this resource.
        /// </summary>
        public string ResourceType { get; private set; }

        /// <summary>
        /// Gets the defined attributes of this resource.
        /// </summary>
        public IEnumerable<ResourceAttribute> Attributes => _attributes;

        /// <summary>
        /// Gets the defined relationships of this resource.
        /// </summary>
        public IEnumerable<ResourceRelationship> Relationships => _relationships;

        /// <summary>
        /// Gets the defined identifier of this resource.
        /// </summary>
        public string IdProperty { get; private set; }

        /// <summary>
        /// Gets the defined <see cref="LinkType"/> to be generated for this resource.
        /// </summary>
        public LinkType LinkType { get; private set; } = LinkType.All;

        /// <summary>
        /// Returns metadata for API responses that serialize this resource.
        /// </summary>
        /// <param name="response">The response object or collection.</param>
        /// <param name="resourceType">
        /// The type of the resource that is being serialized. If
        /// the response is a collection, this is the generic type
        /// parameter of that collection.
        /// </param>
        /// <param name="isEnumerable">True if the response is a collection of items, otherwise false.</param>
        /// <returns>An object that will be serialized into the `meta` hash of the response.</returns>
        public virtual object GetMetadata(object response, Type resourceType, bool isEnumerable)
        {
            return null;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ResourceType;
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
        /// Customize the id property of this resource. The default value
        /// is 'Id'.
        /// </summary>
        /// <param name="name">The name of the property that holds the id.</param>
        /// <returns>Value that was set.</returns>
        protected string WithId(string name)
        {
            VerifyPropertyName(name, allowId: true);

            IdProperty = name.ToPascalCase();

            return IdProperty;
        }

        /// <summary>
        /// Customize the LinkType property of this resource. The default value
        /// is <see cref="LinkType.All"/>.
        /// </summary>
        /// <param name="withLinks">The desired <see cref="LinkType"/> to generate for this resource.</param>
        /// <returns>Value that was set.</returns>
        protected LinkType WithLinks(LinkType withLinks)
        {
            LinkType = withLinks;

            return LinkType;
        }

        /// <summary>
        /// Specify an attribute of this resource.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The <see cref="ResourceAttribute"/>.</returns>
        protected ResourceAttribute Attribute(string name)
        {
            VerifyPropertyName(name);

            var result = new ResourceAttribute(name);

            _attributes.Add(result);

            return result;
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <typeparam name="T">The api resource type of the relationship.</typeparam>
        /// <returns>The <see cref="ResourceRelationship"/>.</returns>
        protected ResourceRelationship BelongsTo<T>(string name)
                    where T : ApiResource, new()
        {
            return BelongsTo<T>(name, name);
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        /// <typeparam name="T">The api resource type of the relationship.</typeparam>
        /// <returns>The <see cref="ResourceRelationship"/>.</returns>
        protected ResourceRelationship BelongsTo<T>(string name, string path)
                    where T : ApiResource, new()
        {
            return BelongsTo<T>(name, path, LinkType.All);
        }

        /// <summary>
        /// Specify a to-one relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default
        /// is the name)</param>
        /// <param name="withLinks">The defined <see cref="LinkType"/> to be generated for this relationship.</param>
        /// <typeparam name="T">The api resource type of the relationship.</typeparam>
        /// <returns>The <see cref="ResourceRelationship"/>.</returns>
        protected ResourceRelationship BelongsTo<T>(string name, string path, LinkType withLinks)
                    where T : ApiResource, new()
        {
            VerifyPropertyName(name);

            var resource = GetUniqueResource<T>();
            var result = new ResourceRelationship<T>(name, path, RelationshipKind.BelongsTo, resource, withLinks);

            _relationships.Add(result);

            return result;
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <typeparam name="T">The api resource type of the relationship.</typeparam>
        /// <returns>The <see cref="ResourceRelationship"/>.</returns>
        protected ResourceRelationship HasMany<T>(string name)
                    where T : ApiResource, new()
        {
            return HasMany<T>(name, name);
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default is the name).</param>
        /// <typeparam name="T">The api resource type of the relationship.</typeparam>
        /// <returns>The <see cref="ResourceRelationship"/>.</returns>
        protected ResourceRelationship HasMany<T>(string name, string path)
                    where T : ApiResource, new()
        {
            return HasMany<T>(name, name, LinkType.All);
        }

        /// <summary>
        /// Specify a to-many relationship of this resource.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="path">The url pathspec of this relationship (default is the name).</param>
        /// <param name="withLinks">The defined <see cref="LinkType"/> to be generated for this relationship.</param>
        /// <typeparam name="T">The api resource type of the relationship.</typeparam>
        /// <returns>The <see cref="ResourceRelationship"/>.</returns>
        protected ResourceRelationship HasMany<T>(string name, string path, LinkType withLinks)
                    where T : ApiResource, new()
        {
            VerifyPropertyName(name);

            var resource = GetUniqueResource<T>();
            var result = new ResourceRelationship<T>(name, path, RelationshipKind.HasMany, resource, withLinks);

            _relationships.Add(result);

            return result;
        }

        private static void VerifyPropertyName(string name, bool allowId = false)
        {
            var dashed = name.ToDashed();

            if (dashed == "id" && !allowId)
            {
                throw new JsonApiException(ErrorType.Server, "You cannot add an attribute named 'id'.");
            }

            if (dashed == "links")
            {
                throw new JsonApiException(ErrorType.Server, "You cannot add an attribute named 'links'.");
            }

            if (dashed == "relationships")
            {
                throw new JsonApiException(ErrorType.Server, "You cannot add an attribute named 'relationships'.");
            }
        }

        private static T GetUniqueResource<T>()
            where T : ApiResource, new()
        {
            var type = typeof(T);
            var resource = Resources.ContainsKey(type)
                ? Resources[type] as T
                : new T();
            return resource;
        }
    }
}
