using CLog.Internal;
using System;

namespace Clog.Test
{
    //
    // 摘要:
    //     Specifies changes to watch for in a file or folder.
    [Flags]
    public enum NotifyFilters
    {
        //
        // 摘要:
        //     The name of the file.
        FileName = 1,
        //
        // 摘要:
        //     The name of the directory.
        DirectoryName = 2,
        //
        // 摘要:
        //     The attributes of the file or folder.
        Attributes = 4,
        //
        // 摘要:
        //     The size of the file or folder.
        Size = 8,
        //
        // 摘要:
        //     The date the file or folder last had anything written to it.
        LastWrite = 16,
        //
        // 摘要:
        //     The date the file or folder was last opened.
        LastAccess = 32,
        //
        // 摘要:
        //     The time the file or folder was created.
        CreationTime = 64,
        //
        // 摘要:
        //     The security settings of the file or folder.
        Security = 256
    }

    internal sealed class MultiFileWatcher
    {
        public NotifyFilters NotifyFilters { get; set; }

        public MultiFileWatcher() :
           this(NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes)
        { }

        public MultiFileWatcher(NotifyFilters notifyFilters)
        {
            NotifyFilters = notifyFilters;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var fileNotity = new MultiFileWatcher();
            //Console.WriteLine(fileNotity.NotifyFilters);
            //var className = new TestBase() { Name = StackTraceUsageUtils.GetClassFullName() };
            //Console.WriteLine("Class Name: {0}", className);
            Console.Read();
        }
    }

    class TestBase
    {
        public string Name;

    }
}
