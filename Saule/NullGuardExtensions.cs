using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saule
{
    internal static class NullGuardExtensions
    {
        internal static T ThrowIfNull<T>(this T obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return obj;
        }
    }
}
