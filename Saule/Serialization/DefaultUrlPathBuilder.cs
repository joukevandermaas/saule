using Humanizer;

namespace Saule.Serialization
{
    /// <summary>
    /// Used to build url paths.
    /// </summary>
    public class DefaultUrlPathBuilder : IUrlPathBuilder
    {
        /// <summary>
        /// Returns the UrlPath of the resource, ensuring it starts and ends with '/'
        /// </summary>
        /// <param name="resource">The resource this path refers to.</param>
        public virtual string BuildCanonicalPath(ApiResource resource)
        {
            return resource.UrlPath.EnsureEndsWith("/");
        }

        /// <summary>
        /// Returns a path in the form `/resource.UrlPath/id/`.
        /// </summary>
        /// <param name="resource">The resource this path refers to.</param>
        /// <param name="id">The unique id of the resource.</param>
        /// <returns></returns>
        public virtual string BuildCanonicalPath(ApiResource resource, string id)
        {
            return '/'.TrimJoin(
                BuildCanonicalPath(resource), id)
                .EnsureEndsWith("/");
        }

        /// <summary>
        /// Returns a path in the form `/resource.UrlPath/id/relationship.UrlPath/`.
        /// </summary>
        /// <param name="resource">The resource this path is related to.</param>
        /// <param name="id">The unique id of the resource.</param>
        /// <param name="relationship">The relationship this path refers to.</param>
        /// <returns></returns>
        public virtual string BuildRelationshipPath(ApiResource resource, string id, ResourceRelationship relationship)
        {
            return '/'.TrimJoin(
                BuildCanonicalPath(resource, id), relationship.UrlPath)
                .EnsureEndsWith("/");
        }

        /// <summary>
        /// Returns a path in the form `/resource.UrlPath/id/relationships/relationship.UrlPath/`.
        /// </summary>
        /// <param name="resource">The resource this path is related to.</param>
        /// <param name="id">The unique id of the resource.</param>
        /// <param name="relationship">The relationship this path refers to.</param>
        /// <param name="relatedResourceId">The id of the related resource.</param>
        public virtual string BuildRelationshipSelfPath(
            ApiResource resource,
            string id,
            ResourceRelationship relationship,
            string relatedResourceId)
        {
            return '/'.TrimJoin(
                BuildCanonicalPath(resource, id), "relationships", relationship.UrlPath)
                .EnsureEndsWith("/");
        }
    }
}