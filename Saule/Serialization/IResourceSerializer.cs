using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal interface IResourceSerializer
    {
        JObject Serialize();

        JObject Serialize(JsonSerializer serializer);
    }
}
