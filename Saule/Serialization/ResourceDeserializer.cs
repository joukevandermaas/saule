using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    /// <summary>
    /// Deserializes json into a specified type of object
    /// </summary>
    internal class ResourceDeserializer
    {
        private static string[] _allowedTopLevelMembers = new[]
        {
            "data", "errors", "meta", "jsonapi", "links", "included"
        };

        private readonly JToken _object;
        private readonly Type _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDeserializer"/> class.
        /// </summary>
        /// <param name="object">Json token we want to convert into an object</param>
        /// <param name="target">The type of object the json token should be converted into</param>
        public ResourceDeserializer(JToken @object, Type target)
        {
            _object = @object;
            _target = target;
        }

        /// <summary>
        /// Deserialize the contained json into the specified type of object
        /// </summary>
        /// <returns>Json converted into the specified type of object</returns>
        public object Deserialize()
        {
            ValidateTopLevel(_object);
            return ToFlatStructure(_object)?.ToObject(_target);
        }

        private static void ValidateTopLevel(JToken content)
        {
            var isObject = content.Type == JTokenType.Object;

            if (!isObject)
            {
                throw new JsonApiException(ErrorType.Client, "Invalid JSON API request content.");
            }

            var objContent = (JObject)content;
            var properties = objContent.Properties().Select(p => p.Name).ToList();

            var hasData = properties.Contains("data");
            var hasErrors = properties.Contains("errors");
            var hasMeta = properties.Contains("meta");
            var hasIncluded = properties.Contains("included");

            if (!(hasData || hasErrors || hasMeta))
            {
                throw new JsonApiException(ErrorType.Client, "Invalid JSON API request content.");
            }

            if (hasData && hasErrors)
            {
                throw new JsonApiException(ErrorType.Client, "Invalid JSON API request content.");
            }

            if (!hasData && hasIncluded)
            {
                throw new JsonApiException(ErrorType.Client, "Invalid JSON API request content.");
            }

            if (!properties.All(p => _allowedTopLevelMembers.Contains(p)))
            {
                throw new JsonApiException(ErrorType.Client, "Invalid JSON API request content.");
            }
        }

        private JToken ToFlatStructure(JToken json)
        {
            var array = json["data"] as JArray;

            if (array == null)
            {
                var obj = json["data"] as JObject;

                if (obj == null)
                {
                    return null;
                }

                return SingleToFlatStructure(json["data"] as JObject);
            }

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
            {
                result["id"] = child["id"];
            }

            foreach (var attr in child["attributes"] ?? new JArray())
            {
                var prop = attr as JProperty;
                result.Add(prop?.Name.ToPascalCase(), prop?.Value);
            }

            foreach (var rel in child["relationships"] ?? new JArray())
            {
                var prop = rel as JProperty;
                result.Add(prop?.Name.ToPascalCase(), ToFlatStructure(prop?.Value));
            }

            return result;
        }
    }
}