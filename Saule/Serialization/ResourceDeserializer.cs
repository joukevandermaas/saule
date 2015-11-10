using System;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ResourceDeserializer
    {
        private readonly JToken _object;
        private readonly Type _target;

        public ResourceDeserializer(JToken @object, Type target)
        {
            _object = @object;
            _target = target;
        }

        public object Deserialize()
        {
            return ToFlatStructure(_object).ToObject(_target);
        }

        private JToken ToFlatStructure(JToken json)
        {
            var array = json["data"] as JArray;

            if (array == null) return SingleToFlatStructure(json["data"] as JObject);

            var result = new JArray();
            foreach (var child in array)
            {
                result.Add(SingleToFlatStructure(child as JObject));
            }

            return result;
        }

        private JToken SingleToFlatStructure(JObject child)
        {
            var result = new JObject();
            if (child["id"] != null)
                result["id"] = child["id"];

            foreach (var attr in child["attributes"] ?? new JArray())
            {
                var prop = attr as JProperty;
                result.Add(prop?.Name.ToPascalCase(), prop?.Value);
            }

            foreach (var rel in child["relationships"] ?? new JArray())
            {
                var prop = rel as JProperty;
                result.Add(prop?.Name, ToFlatStructure(prop?.Value));
            }

            return result;
        }
    }
}