namespace Saule.Resources
{
    /// <summary>
    /// ApiResourceProvider that can resolve specific ApiResource based on the data object
    /// </summary>
    public interface IApiResourceProvider
    {
        /// <summary>
        /// Returns ApiResource based on the data object
        /// </summary>
        /// <param name="dataObject">Data object that is serialized. If it's an array, then it will be called for each item in the array</param>
        /// <returns>ApiResource that should be used for specific data object</returns>
        ApiResource Resolve(object dataObject);

        /// <summary>
        /// Returns ApiResource based on the data object and specific relationship
        /// </summary>
        /// <param name="dataObject">Data object that is serialized. If it's an array, then it will be called for each item in the array</param>
        /// <param name="relationship">Relationship resource for which we are resolving api resource</param>
        /// <returns>ApiResource that should be used for specific data object and relationship</returns>
        ApiResource ResolveRelationship(object dataObject, ApiResource relationship);
    }
}
