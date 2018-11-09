using System;
using System.Collections.Generic;
using System.Text;

namespace CLog.Internal
{
    internal static class ArrayHelper
    {
        private static class EmptyArray<T>
        {
            internal static readonly T[] Instance = new T[0];    
        }

        internal static T[] Empty<T>()
        {
            return EmptyArray<T>.Instance;
        }
    }
}
