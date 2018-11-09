using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CLog.Internal
{
    internal static class ExceptionHelper
    {
        private const string LoggedKey = "CLog.ExceptionLoggedToInternalLogger";

        public static bool MustBeRethrown(this Exception ex)
        {
            if (ex.MustBeRethrownImmediately())
            {
                //没有日志记录，因为它只会使服务器异常变得更糟。
                return true;
            }

            var isConfigError = ex is CLogConfigurationException;

            if (!ex.IsLoggedToInternalLogger())
            {
                var level = isConfigError ? LogLevel.Warn : LogLevel.Error;

            }

            //if ThrowConfigExceptions == null, use  ThrowExceptions
            var shallRethrow = isConfigError ? (LogManager.ThrowConfigExceptions ?? LogManager.ThrowExceptions) : LogManager.ThrowExceptions;
            return shallRethrow;
        }

        public static bool IsLoggedToInternalLogger(this Exception exception)
        {
            if (exception !=null)
            {
                return exception.Data[LoggedKey] as bool? ?? false;
            }
            return false;
        }

        public static bool MustBeRethrownImmediately(this Exception exception)
        {
            if (exception is StackOverflowException)
            {
                return true;
            }

            if(exception is ThreadAbortException)
            {
                return true;
            }

            if(exception is OutOfMemoryException)
            {
                return true;
            }

            return false;
        }
    }
}
