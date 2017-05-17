using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saule.Serialization
{
    internal class ResourceGraphPathSet : IEquatable<ResourceGraphPathSet>
    {
        private readonly HashSet<string> _paths;
        private HashSet<string> _localProperties;

        public ResourceGraphPathSet()
        {
            _paths = new HashSet<string>();
        }

        public ResourceGraphPathSet(IEnumerable<string> paths)
        {
            _paths = new HashSet<string>(paths);
        }

        public IReadOnlyCollection<string> Paths
        {
            get
            {
                return (IReadOnlyCollection<string>)_paths;
            }
        }

        public static bool operator ==(ResourceGraphPathSet k1, ResourceGraphPathSet k2)
        {
            return k1.Equals(k2);
        }

        public static bool operator !=(ResourceGraphPathSet k1, ResourceGraphPathSet k2)
        {
            return !k1.Equals(k2);
        }

        public virtual bool MatchesProperty(string property)
        {
            property.ThrowIfNull(nameof(property));

            if (_localProperties == null)
            {
                _localProperties = new HashSet<string>(_paths.Select(i => i.SubstringToSeperator(".")));
            }

            return _localProperties.Contains(property);
        }

        public virtual ResourceGraphPathSet PathSetForChildProperty(string property)
        {
            property.ThrowIfNull(nameof(property));

            var prefix = $"{property}.";
            return new ResourceGraphPathSet(_paths
                .Where(i => i.StartsWith(prefix))
                .Select(i => i.Substring(prefix.Length)));
        }

        public virtual bool Equals(ResourceGraphPathSet other)
        {
            return ReferenceEquals(this, other) || _paths.SetEquals(other?._paths);
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceGraphPathSet ? Equals((ResourceGraphPathSet)obj) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _paths.GetHashCode();
        }

        public virtual ResourceGraphPathSet Union(ResourceGraphPathSet other)
        {
            other.ThrowIfNull(nameof(other));

            var result = new HashSet<string>(_paths);
            result.UnionWith(other._paths);
            return new ResourceGraphPathSet(result);
        }

        internal class All : ResourceGraphPathSet
        {
            public override bool MatchesProperty(string property)
            {
                return true;
            }

            public override ResourceGraphPathSet PathSetForChildProperty(string property)
            {
                return this;
            }

            public override bool Equals(ResourceGraphPathSet other)
            {
                return other is All;
            }

            public override bool Equals(object obj)
            {
                return obj is All ? Equals((All)obj) : base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }

        internal class None : ResourceGraphPathSet
        {
            public override bool MatchesProperty(string property)
            {
                return false;
            }

            public override ResourceGraphPathSet PathSetForChildProperty(string property)
            {
                return this;
            }

            public override bool Equals(ResourceGraphPathSet other)
            {
                return other is None;
            }

            public override bool Equals(object obj)
            {
                return obj is None ? Equals((None)obj) : base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return 2;
            }
        }
    }
}
