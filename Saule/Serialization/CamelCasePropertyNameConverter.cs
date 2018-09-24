namespace Saule.Serialization
{
    /// <summary>
    /// Converts property names using camelCase notation for
    /// json properties and PascalCase for model properties
    /// </summary>
    public class CamelCasePropertyNameConverter : IPropertyNameConverter
    {
        /// <inheritdoc/>
        public virtual string ToModelPropertyName(string name)
        {
            return name.ToPascalCase();
        }

        /// <inheritdoc/>
        public virtual string ToJsonPropertyName(string name)
        {
            return name.ToCamelCase();
        }
    }
}
