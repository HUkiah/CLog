using System;
using System.Collections.Generic;
using System.Text;

namespace CLog.Internal
{
    internal static class ReflectionHelpers
    {
        public static bool IsStaticClass(this Type type)
        {
            return type.IsClass() && type.IsAbstract() && type.IsSealed();
        }

        public static bool IsClass(this Type type)
        {
            return type.IsClass;
        }

        public static bool IsAbstract(this Type type)
        {
            return type.IsAbstract;
        }

        public static bool IsSealed(this Type type)
        {
            return type.IsSealed;
        }
    }
}
