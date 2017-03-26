namespace Saule
{
    /// <summary>
    /// Represents an attribute on a resource.
    /// </summary>
    public class ResourceAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the attribute. Will be normalized to the JSON API dashed name convention.</param>
        public ResourceAttribute(string name)
        {
            Name = name.ToDashed();
            OriginalName = name;
        }

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the original name without normalization for traceability.
        /// </summary>
        public string OriginalName { get; }
    }
}