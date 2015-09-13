using Newtonsoft.Json.Linq;
using System;

namespace Saule.Serialization
{
    internal class JsonApiDeserializer
    {
        public object Deserialize(JToken json, Type targetType)
        {
            return ToFlatStructure(json).ToObject(targetType);
        }

        private JToken ToFlatStructure(JToken json)
        {
            var array = json["data"] as JArray;
            if (array != null)
            {
                var result = new JArray();

                foreach (var child in array)
                {
                    result.Add(SingleToFlatStructure(child as JObject));
                }
                return result;
            }
            else
            {
                return SingleToFlatStructure(json["data"] as JObject);
            }
        }

        private JToken SingleToFlatStructure(JObject child)
        {
            var result = new JObject();
            result.Add(child.Property("id"));

            foreach (var attr in child["attributes"] ?? new JArray())
                result.Add(attr);

            foreach (var rel in child["relationships"] ?? new JArray())
                result.Add((rel as JProperty).Name, ToFlatStructure(rel.First));

            return result;
        }
    }
}