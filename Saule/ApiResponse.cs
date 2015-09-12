using System;
using Newtonsoft.Json.Linq;

namespace Saule
{
    public class ApiResponse<T>
    {
        private ApiModel _model;
        private T _object;
        private JObject _objectJson;

        public ApiResponse(T obj, ApiModel model)
        {
            _object = obj;
            _objectJson = JObject.FromObject(_object);
            _model = model;
        }

        public JObject ToJson()
        {
            var data = new JObject();
            data["type"] = _model.ModelType.ToDashed();
            data["id"] = ResolveAttributeValue("id");

            data["attributes"] = GetAttributes();

            return new JObject { { "data", data } };
        }

        private JToken GetAttributes()
        {
            var attributes = new JObject();
            foreach (var attr in _model.Attributes)
            {
                attributes.Add(attr.Name, ResolveAttributeValue(attr.Name));
            }

            return attributes;
        }

        private JToken ResolveAttributeValue(string name)
        {
            return _objectJson[name.ToPascalCase()];
        }
    }
}