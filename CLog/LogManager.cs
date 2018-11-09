namespace CLog
{
    using CLog.Internal;
    using System;
    using System.Runtime.CompilerServices;

    public static class LogManager
    {
        internal static readonly LogFactory factory = new LogFactory();

        public static bool ThrowExceptions
        {
            get => factory.ThrowExceptions;
            set => factory.ThrowExceptions = value;
        }

        public static bool? ThrowConfigExceptions
        {
            get => factory.ThrowConfigExceptions;
            set => factory.ThrowConfigExceptions = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger GetCurrentClassLogger()
        {
            return factory.GetLogger(StackTraceUsageUtils.GetClassFullName());
        }


    }
}
