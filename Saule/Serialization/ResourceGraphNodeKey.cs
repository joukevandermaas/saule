using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal struct ResourceGraphNodeKey : IEquatable<ResourceGraphNodeKey>
    {
        public ResourceGraphNodeKey(
            object obj,
            ApiResource resource)
        {
            obj.ThrowIfNull(nameof(obj));
            resource.ThrowIfNull(nameof(resource));

            Type = resource.ResourceType.ToDashed();
            if (obj != null)
            {
                Id = obj.GetValueOfProperty(resource.IdProperty)?.ToString();
            }
            else
            {
                /* To silence compiler warning about returning with Id unassigned,
                 * despite us throwing an exception before that ever happens.
                 */
                Id = null;
            }

            if (Id == null)
            {
                throw new JsonApiException(ErrorType.Server, "Resources must have an id");
            }
        }

        public ResourceGraphNodeKey(
            string type,
            string id)
        {
            type.ThrowIfNull(nameof(type));
            id.ThrowIfNull(nameof(id));

            Type = type;
            Id = id;
        }

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("id")]
        public string Id { get; private set; }

        public static bool operator ==(ResourceGraphNodeKey k1, ResourceGraphNodeKey k2)
        {
            return k1.Equals(k2);
        }

        public static bool operator !=(ResourceGraphNodeKey k1, ResourceGraphNodeKey k2)
        {
            return !k1.Equals(k2);
        }

        public bool Equals(ResourceGraphNodeKey other)
        {
            return Type.Equals(other.Type) && Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceGraphNodeKey ? Equals((ResourceGraphNodeKey)obj) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Id.GetHashCode();
        }
    }
}
