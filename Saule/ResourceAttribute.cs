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
        /// <param name="name">The name of the attribute.</param>
        public ResourceAttribute(string name)
        {
            Name = name.ToDashed();
        }

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        public string Name { get; }
    }
}