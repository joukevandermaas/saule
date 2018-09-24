namespace Saule.Serialization
{
    /// <summary>
    /// Used to convert property names
    /// </summary>
    public interface IPropertyNameConverter
    {
        /// <summary>
        /// Converts a json property name to a model property name
        /// </summary>
        /// <param name="name">json property name</param>
        /// <returns>model property name</returns>
        string ToModelPropertyName(string name);

        /// <summary>
        /// Converts a model property name to a json property name
        /// </summary>
        /// <param name="name">model property name</param>
        /// <returns>json property name</returns>
        string ToJsonPropertyName(string name);
    }
}
