using System;

namespace AVS.CoreLib.Utilities
{
    public static class ReflectionUtils
    {
        public static bool IsNullable(Type type)
        {
            if (type.IsValueType)
            {
                return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
            }

            return true;
        }
    }
}