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
    }
}
