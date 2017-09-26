using Newtonsoft.Json.Serialization;

namespace Saule.Serialization
{
    internal class JsonApiContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToDashed();
        }
    }
}
