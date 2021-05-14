﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Saule
{
    internal static class ObjectExtensions
    {
        public static object GetValueOfProperty(this object obj, string propertyName, bool searchForInternals = false)
        {
            if (obj == null || propertyName == null)
            {
                return null;
            }

            if (obj is IDictionary<string, object>)
            {
                var dict = obj as IDictionary<string, object>;
                if (dict.ContainsKey(propertyName))
                {
                    return dict[propertyName];
                }
            }

            if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                if (dict.Contains(propertyName))
                {
                    return dict[propertyName];
                }
            }

            BindingFlags flags = searchForInternals
                ? BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic

                // these are default binding flags based from source
                // https://referencesource.microsoft.com/#mscorlib/system/type.cs,719
                : BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

            var propertyInfo = obj.GetType().GetProperty(propertyName, flags);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj);
            }

            var fieldInfo = obj.GetType().GetField(propertyName, flags);
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }

            return null;
        }

        public static bool IncludesProperty(this object obj, string propertyName)
        {
            if (obj == null || propertyName == null)
            {
                return false;
            }

            if (obj is IDictionary<string, object>)
            {
                var dict = obj as IDictionary<string, object>;
                if (dict.ContainsKey(propertyName))
                {
                    return true;
                }
            }

            if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                if (dict.Contains(propertyName))
                {
                    return true;
                }
            }

            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return true;
            }

            var fieldInfo = obj.GetType().GetField(propertyName);
            if (fieldInfo != null)
            {
                return true;
            }

            return false;
        }

        public static bool IsDictionaryType(this object obj)
        {
            return obj is IDictionary ||
                obj is IDictionary<string, object>;
        }

        public static bool IsCollectionType(this object obj)
        {
            return !obj.IsDictionaryType() &&
                obj is IEnumerable;
        }
    }
}
