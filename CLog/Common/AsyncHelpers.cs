namespace CLog.Common
{
    using System;
    using System.Threading;

    public static class AsyncHelpers
    {
        internal static bool WaitForDispose(this Timer timer,TimeSpan timeout)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);

            if (timeout != TimeSpan.Zero)
            {
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                if (timer.Dispose(waitHandle) && !waitHandle.WaitOne((int)timeout.TotalMilliseconds))
                {
                    return false;
                }
                waitHandle.Close();
            }
            else
            {
                timer.Dispose();
            }

            return true;
        }

        //public static void RunSynchronously(AsynchronousAction action)
        //{
        //    var ev = new ManualResetEvent(false);
        //    Exception lastException = null;
        //    action(PreventMultipleCalls)

        //}

        //public static 
    }
}
