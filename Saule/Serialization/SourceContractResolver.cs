using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Saule.Serialization
{
    internal class SourceContractResolver : DefaultContractResolver
    {
        private readonly IPropertyNameConverter _nameConverter;
        private readonly ApiResource _apiResource;
        private int _depth;

        public SourceContractResolver(IPropertyNameConverter nameConverter, ApiResource apiResource)
        {
            _apiResource = apiResource;
            _nameConverter = nameConverter;
            _depth = 0;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            if (_apiResource != null && _depth == 0)
            {
                // Only filter out properties at the root level and when given a api resource that list needed field
                properties = properties.Where(property => _apiResource.Attributes.Any(att => att.PropertyName == property.PropertyName)).ToList();
            }

            foreach (var property in properties)
            {
                property.PropertyName = _nameConverter.ToJsonPropertyName(property.PropertyName);
            }

            return properties;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            contract.OnSerializingCallbacks.Add((obj, context) => { _depth++; });
            contract.OnSerializedCallbacks.Add((obj, context) => { _depth--; });

            return contract;
        }
    }
}
