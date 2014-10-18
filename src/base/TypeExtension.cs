using System;

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
    }
}
