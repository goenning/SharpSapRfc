using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpSapRfc
{
    public static class TypeExtension
    {
        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType) 
                return true;

            if (Nullable.GetUnderlyingType(type) != null)
                return true;

            return false;
        }

        public static bool IsEnumerable(this Type type) 
        { 
            if (type.Equals(typeof(string)))
                return false;

            return type.GetInterfaces().Any(
                ti => ti.FullName.Equals("System.Collections.IEnumerable")
            );
        }

        public static Type GetEnumerableInnerType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            return type.GetGenericArguments()[0];
        }

        public static bool IsComplexType(this Type type) 
        {
            return !(type.IsPrimitive || type.IsValueType || type == typeof(string));
        }
    }
}
