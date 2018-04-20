namespace Saule.Serialization
{
    /// <summary>
    /// Converts property names using kebab/dashed notation for
    /// json properties and PascalCase for model properties
    /// </summary>
    public class DefaultPropertyNameConverter : IPropertyNameConverter
    {
        /// <inheritdoc/>
        public virtual string ToModelPropertyName(string name)
        {
            return name.ToPascalCase();
        }

        /// <inheritdoc/>
        public virtual string ToJsonPropertyName(string name)
        {
            return name.ToDashed();
        }
    }
}
