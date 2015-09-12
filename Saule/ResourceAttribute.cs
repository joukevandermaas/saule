namespace Saule
{
    /// <summary>
    /// Represents an attribute on a resource.
    /// </summary>
    public class ResourceAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        public ResourceAttribute(string name)
        {
            Name = name.ToCamelCase();
        }

        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public string Name { get; }
    }
}