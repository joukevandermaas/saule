using System;
using System.Collections.Generic;
using System.Linq;
using Saule.Queries;

namespace Saule
{
    internal static class TypeExtensions
    {
        public static object CreateInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static T CreateInstance<T>(this Type type)
            where T : class
        {
            return Activator.CreateInstance(type) as T;
        }

        public static IEnumerable<Type> GetInheritanceChain(this Type type)
        {
            if (type.BaseType == null)
            {
                return type.ToEnumerable()
                    .Concat(type.GetInterfaces());
            }

            return type.ToEnumerable()
                .Concat(type.GetInterfaces())
                .Concat(type.BaseType.ToEnumerable())
                .Concat(type.GetInterfaces().SelectMany(GetInheritanceChain))
                .Concat(type.BaseType.GetInheritanceChain())
                .Distinct();
        }
    }
}