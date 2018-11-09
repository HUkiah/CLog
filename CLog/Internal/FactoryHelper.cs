namespace CLog.Internal
{
    using System;

    internal class FactoryHelper
    {
        private FactoryHelper() { }

        internal static object CreateInstance(Type t)
        {
            try
            {
                return Activator.CreateInstance(t);

            } catch (MissingMethodException ex)
            {
                throw new CLogConfigurationException($" 不能访问'{t.FullName}'的构造函数，是否有所需的许可？",ex);
            }
        }

    }
}
