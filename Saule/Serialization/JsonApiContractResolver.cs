using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Saule.Serialization
{
    internal class JsonApiContractResolver : DefaultContractResolver
    {
        private readonly IPropertyNameConverter _nameConverter;

        public JsonApiContractResolver(IPropertyNameConverter nameConverter)
        {
            _nameConverter = nameConverter;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            foreach (var property in properties)
            {
                property.PropertyName = _nameConverter.ToJsonPropertyName(property.PropertyName);
            }

            return properties;
        }
    }
}
