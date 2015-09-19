using System;

namespace Saule
{
    internal static class TypeExtensions
    {
        public static object CreateInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }
        public static T CreateInstance<T>(this Type type) where T : class
        {
            return Activator.CreateInstance(type) as T;
        }
    }
}