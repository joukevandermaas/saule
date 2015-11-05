using System;

namespace Saule.Serialization
{
    /// <summary>
    ///     Always builds canonical self paths, rather than relationship paths.
    /// </summary>
    public class CanonicalUrlPathBuilder : DefaultUrlPathBuilder
    {
        /// <summary>
        ///     Create instance
        /// </summary>
        /// <param name="basePath">
        ///     <example>"/api"</example>
        /// </param>
        public CanonicalUrlPathBuilder(string basePath = "/")
        {
            if (string.IsNullOrEmpty(basePath) || basePath[0] != '/')
                throw new ArgumentException("Canonical base path must start with '/'.", nameof(basePath));
            BasePath = basePath;
        }

        /// <summary>
        ///     Base path for all canonical urls.
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        ///     Returns the UrlPath of the resource, ensuring it starts and ends with '/'
        /// </summary>
        /// <param name="resource">The resource this path refers to.</param>
        public override string BuildCanonicalPath(ApiResource resource)
        {
            return base.BuildCanonicalPath(resource).EnsureStartsWith("/").EnsureStartsWith(BasePath);
        }

        /// <summary>
        ///     Returns a path in the form `/resource.UrlPath/id/`.
        /// </summary>
        /// <param name="resource">The resource this path refers to.</param>
        /// <param name="id">The unique id of the resource.</param>
        /// <returns></returns>
        public override string BuildCanonicalPath(ApiResource resource, string id)
        {
            return base.BuildCanonicalPath(resource, id).EnsureStartsWith("/").EnsureStartsWith(BasePath);
        }

        /// <summary>
        ///     Returns a path in the form `/resource.UrlPath/id/relationship.UrlPath/`.
        /// </summary>
        /// <param name="resource">The resource this path is related to.</param>
        /// <param name="id">The unique id of the resource.</param>
        /// <param name="relationship">The relationship this path refers to.</param>
        /// <returns></returns>
        public override string BuildRelationshipPath(ApiResource resource, string id, ResourceRelationship relationship)
        {
            return
                base.BuildRelationshipPath(resource, id, relationship).EnsureStartsWith("/").EnsureStartsWith(BasePath);
        }

        /// <summary>
        ///     Returns a path in the form `/resource.UrlPath/id/relationships/relationship.UrlPath/`.
        /// </summary>
        /// <param name="resource">The resource this path is related to.</param>
        /// <param name="id">The unique id of the resource.</param>
        /// <param name="relationship">The relationship this path refers to.</param>
        /// <param name="relatedResourceId">The id of the related resource.</param>
        /// <returns></returns>
        public override string BuildRelationshipSelfPath(
            ApiResource resource,
            string id,
            ResourceRelationship relationship,
            string relatedResourceId)
        {
            var s = base.BuildRelationshipSelfPath(resource, id, relationship, relatedResourceId)
                    .EnsureStartsWith("/")
                    .EnsureStartsWith(BasePath);
            return s;

        }
    }
}