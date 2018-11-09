using CLog.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLog.Internal
{
    internal interface ISupportsInitialize
    {
        void Initialize(LoggingConfiguration configuration);
        void Close();
    }
}
