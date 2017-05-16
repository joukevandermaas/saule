using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saule
{
    internal static class ObjectExtensions
    {
        public static object GetValueOfProperty(this object obj, string propertyName)
        {
            if (obj == null || propertyName == null)
            {
                return null;
            }

            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj);
            }

            var fieldInfo = obj.GetType().GetField(propertyName);
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
    }
}
