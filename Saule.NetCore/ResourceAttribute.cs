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
            PropertyName = name.ToPascalCase();
        }

        /// <summary>
        /// Gets the name of the attribute in dashed JSON API format.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the attribute in PascalCase.
        /// </summary>
        public string PropertyName { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return PropertyName;
        }
    }
}