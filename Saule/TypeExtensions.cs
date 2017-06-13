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

        public static bool IsEnumerable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        /// <summary>
        /// Get the type of the items if the type is an <see cref="IEnumerable{T}"/> (if not it will return null).
        /// </summary>
        /// <param name="type">The type to apply this extension method to.</param>
        /// <returns>The type of the items if the type is an <see cref="IEnumerable{T}"/> (if not it will return null).</returns>
        public static Type GetGenericTypeParameterOfCollection(this Type type)
        {
            var collectionType = type.GetInterfaces().FirstOrDefault(i => i.IsEnumerable());
            return collectionType?.GenericTypeArguments[0];
        }
    }
}