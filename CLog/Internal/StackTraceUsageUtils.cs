using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CLog.Internal
{
    internal static class StackTraceUsageUtils
    {
        private static readonly Assembly clogAssembly = typeof(StackTraceUsageUtils).GetAssembly();
        private static readonly Assembly systemAssembly = typeof(Debug).GetAssembly();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetClassFullName()
        {
            int frameToSkip = 2;
            string className = string.Empty;

            var stackFrame = new StackFrame(frameToSkip, false);
            className = GetClassFullName(stackFrame);

            return className;
        }

        public static string GetClassFullName(StackFrame stackFrame)
        {
            string className = LookupClassNameFromStackFrame(stackFrame);
            if (string.IsNullOrEmpty(className))
            {
                var stackTrace = new StackTrace(false);

                className = GetClassFullName(stackTrace);
            }

            return className;
        }

        private static string GetClassFullName(StackTrace stackTrace)
        {
            foreach (StackFrame frame in stackTrace.GetFrames())
            {
                string className = LookupClassNameFromStackFrame(frame);

                if (!string.IsNullOrEmpty(className))
                {
                    return className;
                }
            }
            return string.Empty;
        }

        public static string LookupClassNameFromStackFrame(StackFrame stackFrame)
        {
            var method = stackFrame.GetMethod();
            if (method!=null&&LookupAssemblyFromStackFrame(stackFrame)!=null)
            {
                string className = GetStackFrameMethodClassName(method, true, true, true) ?? method.Name;
                if (!string.IsNullOrEmpty(className) && !className.StartsWith("System.", StringComparison.Ordinal))
                {
                    return className;
                }
            }

            return string.Empty;
        }

        public static string GetStackFrameMethodClassName(MethodBase  method, bool includeNameSpace,bool cleanAsyncMoveNext,bool cleanAnonymousDelegates)
        {
            if (method == null)
                return null;

            var callerClassType = method.DeclaringType;

            if (cleanAsyncMoveNext && method.Name == "MoveNext" && callerClassType?.DeclaringType != null && callerClassType.Name.StartsWith("<"))
            {
                int endIndex = callerClassType.Name.IndexOf('>', 1);
                if (endIndex > 1)
                {
                    callerClassType = callerClassType.DeclaringType;
                }
            }

            if (!includeNameSpace && callerClassType?.DeclaringType != null && callerClassType.IsNested && callerClassType.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
            {
                return callerClassType.DeclaringType.Name;
            }

            string className = includeNameSpace ? callerClassType?.FullName : callerClassType?.Name;

            if (cleanAnonymousDelegates && className!=null)
            {
                int index = className.IndexOf("+<>", StringComparison.Ordinal);

                if (index >= 0)
                {
                    className = className.Substring(0, index);
                }
            }
            return className;
        }

        public static Assembly LookupAssemblyFromStackFrame(StackFrame stackFrame)
        {
            var method = stackFrame.GetMethod();
            if (method == null)
            {
                return null;
            }

            var assembly = method.DeclaringType?.GetAssembly() ?? method.Module.Assembly;
            if (assembly == clogAssembly)
            {
                return null;
            }

            if (assembly == systemAssembly)
            {
                return null;
            }

            return assembly;
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.Assembly;
        }
    }
}
