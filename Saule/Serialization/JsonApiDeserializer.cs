using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Saule.Serialization
{
    class JsonApiDeserializer
    {
        public T Deserialize<T>(JToken json)
        {
            return ToFlatStructure(json).ToObject<T>();
        }

        private JToken ToFlatStructure(JToken json)
        {
            var array = json["data"] as JArray;
            if(array != null)
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
