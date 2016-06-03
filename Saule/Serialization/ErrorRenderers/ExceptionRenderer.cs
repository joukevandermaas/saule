using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization.ErrorRenderers
{
    internal class ExceptionRenderer
    {

        private ApiError _error;

        public ExceptionRenderer(ApiError error)
        {
            _error = error;
        }

        public ExceptionRenderer()
        {
        }

        public virtual JArray ToJObject()
        {
            var result = new JObject();
            var json = JObject.FromObject(
               _error,
               new JsonSerializer
               {
                   NullValueHandling = NullValueHandling.Ignore
               });

            foreach (var token in json)
            {
                result.Add(token.Key.ToCamelCase(), token.Value);
            }

            return new JArray { result };
        }
    }
}
